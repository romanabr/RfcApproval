﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Data;
using System.Collections;
using RedSys.RFC.Core.Helper;

namespace RedSys.Common.Workflow
{
    public class CompareObjects
    {
        #region Class Variables

        /// <summary>
        /// Keep track of parent objects in the object hiearchy
        /// </summary>
        protected readonly List<object> _parents = new List<object>();

        /// <summary>
        /// Reflection Cache for property info
        /// </summary>
        protected readonly Dictionary<Type, PropertyInfo[]> _propertyCache = new Dictionary<Type, PropertyInfo[]>();

        /// <summary>
        /// Reflection Cache for field info
        /// </summary>
        protected readonly Dictionary<Type, FieldInfo[]> _fieldCache = new Dictionary<Type, FieldInfo[]>();
        #endregion

        #region Properties

        /// <summary>
        /// Ignore classes, properties, or fields by name during the comparison.
        /// Case sensitive.
        /// </summary>
        /// <example>ElementsToIgnore.Add("CreditCardNumber")</example>
        public List<string> ElementsToIgnore { get; set; }

        /// <summary>
        /// If true, protected properties and fields will be compared. The default is false.
        /// </summary>
        public bool CompareprotectedProperties { get; set; }

        /// <summary>
        /// If true, protected fields will be compared. The default is false.
        /// </summary>
        public bool CompareprotectedFields { get; set; }

        /// <summary>
        /// If true, static properties will be compared.  The default is true.
        /// </summary>
        public bool CompareStaticProperties { get; set; }

        /// <summary>
        /// If true, static fields will be compared.  The default is true.
        /// </summary>
        public bool CompareStaticFields { get; set; }

        /// <summary>
        /// If true, child objects will be compared. The default is true. 
        /// If false, and a list or array is compared list items will be compared but not their children.
        /// </summary>
        public bool CompareChildren { get; set; }

        /// <summary>
        /// If true, compare read only properties (only the getter is implemented).
        /// The default is true.
        /// </summary>
        public bool CompareReadOnly { get; set; }

        /// <summary>
        /// If true, compare fields of a class (see also CompareProperties).
        /// The default is true.
        /// </summary>
        public bool CompareFields { get; set; }

        /// <summary>
        /// If true, compare properties of a class (see also CompareFields).
        /// The default is true.
        /// </summary>
        public bool CompareProperties { get; set; }

        /// <summary>
        /// The maximum number of differences to detect
        /// </summary>
        /// <remarks>
        /// Default is 1 for performance reasons.
        /// </remarks>
        public int MaxDifferences { get; set; }

        /// <summary>
        /// The differences found during the compare
        /// </summary>
        public List<String> Differences { get; set; }

        /// <summary>
        /// The differences found in a string suitable for a textbox
        /// </summary>
        public string DifferencesString
        {
            get
            {
                StringBuilder sb = new StringBuilder(4096);

                sb.Append("\r\nBegin Differences:\r\n");

                foreach (string item in Differences)
                {
                    sb.AppendFormat("{0}\r\n", item);
                }

                sb.AppendFormat("End Differences (Maximum of {0} differences shown).", MaxDifferences);

                return sb.ToString();
            }
        }

        /// <summary>
        /// Reflection properties and fields are cached. By default this cache is cleared after each compare.  Set to false to keep the cache for multiple compares.
        /// </summary>
        /// <seealso cref="Caching"/>
        /// <seealso cref="ClearCache"/>
        public bool AutoClearCache { get; set; }

        /// <summary>
        /// By default properties and fields for types are cached for each compare.  By default this cache is cleared after each compare.
        /// </summary>
        /// <seealso cref="AutoClearCache"/>
        /// <seealso cref="ClearCache"/>
        public bool Caching { get; set; }

        /// <summary>
        /// A list of attributes to ignore a class, property or field
        /// </summary>
        /// <example>AttributesToIgnore.Add(typeof(XmlIgnoreAttribute));</example>
        public List<Type> AttributesToIgnore { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Set up defaults for the comparison
        /// </summary>
        public CompareObjects()
        {
            Differences = new List<string>();
            ElementsToIgnore = new List<string>();
            AttributesToIgnore = new List<Type>();
            CompareStaticFields = true;
            CompareStaticProperties = true;
            CompareprotectedProperties = false;
            CompareprotectedFields = false;
            CompareChildren = true;
            CompareReadOnly = true;
            CompareFields = true;
            CompareProperties = true;
            Caching = true;
            AutoClearCache = true;
            MaxDifferences = 1;
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Compare two objects of the same type to each other.
        /// </summary>
        /// <remarks>
        /// Check the Differences or DifferencesString Properties for the differences.
        /// Default MaxDifferences is 1 for performance
        /// </remarks>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <returns>True if they are equal</returns>
        public bool Compare(object object1, object object2)
        {
            string defaultBreadCrumb = string.Empty;

            Differences.Clear();
            Compare(object1, object2, defaultBreadCrumb);

            if (AutoClearCache)
                ClearCache();

            return Differences.Count == 0;
        }

        public bool Compare(object object1, string object1type, string comparetype, string value)
        {
            bool retBool = false;

            if (comparetype == "IsNull")
            {
                retBool = (object1 == null) ? true : false;
                return retBool;
            }
            else if (comparetype == "IsNotNull")
            {
                retBool = (object1 != null) ? true : false;
                return retBool;
            }
            if (object1 == null)
                return false;

            Type t1 = object1.GetType();

            if (IsInt(t1))
            {
                retBool = CompareInt(object1, object1type, comparetype, value);
            }
            else if (IsDouble(t1))
            {
                retBool = CompareDouble(object1, object1type, comparetype, value);
            }
            else if (IsDecimal(t1))
            {
                retBool = CompareDecimal(object1, object1type, comparetype, value);
            }
            else if (IsBool(t1))
            {
                retBool = CompareBool(object1, object1type, comparetype, value);
            }
            else
            {
                retBool = CompareString(object1, object1type, comparetype, value);
            }
            return retBool;
        }

        protected bool CompareString(object object1, string object1type, string comparetype, string value)
        {
            bool retBool = false;
            string i1 = object1.ToString();
            string i2 = value;

            if (i1.Contains(";#"))
                i1 = i1.Substring(i1.IndexOf(";#") + 2);
            else if (i1.Contains("#;"))
                i1 = i1.Substring(i1.IndexOf("#;") + 2);
            switch (comparetype)
            {
                case ("Eq"): retBool = (i1 == i2) ? true : false; break;
                case ("Neq"): retBool = (i1 != i2) ? true : false; break;
                case ("Contains"): retBool = (i1.Contains(i2)) ? true : false; break;
                case ("BeginsWith"): retBool = (i1.StartsWith(i2)) ? true : false; break;
                default: break;
            }

            return retBool;
        }

        protected bool CompareInt(object object1, string object1type, string comparetype, string value)
        {
            bool retBool = false;
            int i1 = Convert.ToInt32(object1);
            int i2 = Convert.ToInt32(value);

            switch (comparetype)
            {
                case ("Eq"): retBool = (i1 == i2) ? true : false; break;
                case ("Geq"): retBool = (i1 >= i2) ? true : false; break;
                case ("Leq"): retBool = (i1 <= i2) ? true : false; break;
                case ("Lt"): retBool = (i1 < i2) ? true : false; break;
                case ("Gt"): retBool = (i1 > i2) ? true : false; break;
                case ("Neq"): retBool = (i1 != i2) ? true : false; break;
                default: break;
            }

            return retBool;
        }

        protected bool CompareBool(object object1, string object1type, string comparetype, string value)
        {
            bool retBool = false;
            bool i1 = Convert.ToBoolean(object1);
            bool i2 = value.IsTrue();

            switch (comparetype)
            {
                case ("Eq"): retBool = (i1 == i2) ? true : false; break;
                case ("Neq"): retBool = (i1 != i2) ? true : false; break;
                default: break;
            }

            return retBool;
        }

        protected bool CompareDouble(object object1, string object1type, string comparetype, string value)
        {
            bool retBool = false;
            double i1 = Convert.ToDouble(object1);
            double i2 = Convert.ToDouble(value);
            switch (comparetype)
            {
                case ("Eq"): retBool = (i1 == i2) ? true : false; break;
                case ("Geq"): retBool = (i1 >= i2) ? true : false; break;
                case ("Leq"): retBool = (i1 <= i2) ? true : false; break;
                case ("Lt"): retBool = (i1 < i2) ? true : false; break;
                case ("Gt"): retBool = (i1 > i2) ? true : false; break;
                case ("Neq"): retBool = (i1 != i2) ? true : false; break;
                default: break;
            }

            return retBool;
        }

        protected bool CompareDecimal(object object1, string object1type, string comparetype, string value)
        {
            bool retBool = false;
            decimal i1 = Convert.ToDecimal(object1);
            decimal i2 = Convert.ToDecimal(value);
            switch (comparetype)
            {
                case ("Eq"): retBool = (i1 == i2) ? true : false; break;
                case ("Geq"): retBool = (i1 >= i2) ? true : false; break;
                case ("Leq"): retBool = (i1 <= i2) ? true : false; break;
                case ("Lt"): retBool = (i1 < i2) ? true : false; break;
                case ("Gt"): retBool = (i1 > i2) ? true : false; break;
                case ("Neq"): retBool = (i1 != i2) ? true : false; break;
                default: break;
            }

            return retBool;
        }

        protected bool IsInt(Type t)
        {
            return t == typeof(int);
        }

        protected bool IsString(Type t)
        {
            return t == typeof(string);
        }

        protected bool IsDouble(Type t)
        {
            return t == typeof(double);
        }

        protected bool IsDecimal(Type t)
        {
            return t == typeof(decimal);
        }

        protected bool IsBool(Type t)
        {
            return t == typeof(bool);
        }
        /// <summary>
        /// Reflection properties and fields are cached. By default this cache is cleared automatically after each compare.
        /// </summary>
        /// <seealso cref="AutoClearCache"/>
        /// <seealso cref="Caching"/>
        public void ClearCache()
        {
            _propertyCache.Clear();
            _fieldCache.Clear();
        }

        #endregion

        #region protected Methods

        /// <summary>
        /// Compare two objects
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="breadCrumb">Where we are in the object hiearchy</param>
        protected void Compare(object object1, object object2, string breadCrumb)
        {
            //If both null return true
            if (object1 == null && object2 == null)
                return;

            //Check if one of them is null
            if (object1 == null)
            {
                Differences.Add(string.Format("object1{0} == null && object2{0} != null ((null),{1})", breadCrumb, cStr(object2)));
                return;
            }

            if (object2 == null)
            {
                Differences.Add(string.Format("object1{0} != null && object2{0} == null ({1},(null))", breadCrumb, cStr(object1)));
                return;
            }

            Type t1 = object1.GetType();
            Type t2 = object2.GetType();

            //Objects must be the same type
            if (t1 != t2)
            {
                Differences.Add(string.Format("Different Types:  object1{0}.GetType() != object2{0}.GetType()", breadCrumb));
                return;
            }

            if (IsDataset(t1))
            {
                CompareDataset(object1, object2, breadCrumb);
            }
            else if (IsDataTable(t1))
            {
                CompareDataTable(object1, object2, breadCrumb);
            }
            else if (IsDataRow(t1))
            {
                CompareDataRow(object1, object2, breadCrumb);
            }
            else if (IsIList(t1)) //This will do arrays, multi-dimensional arrays and generic lists
            {
                CompareIList(object1, object2, breadCrumb);
            }
            else if (IsIDictionary(t1))
            {
                CompareIDictionary(object1, object2, breadCrumb);
            }
            else if (IsEnum(t1))
            {
                CompareEnum(object1, object2, breadCrumb);
            }
            else if (IsPointer(t1))
            {
                ComparePointer(object1, object2, breadCrumb);
            }
            else if (IsSimpleType(t1))
            {
                CompareSimpleType(object1, object2, breadCrumb);
            }
            else if (IsClass(t1))
            {
                CompareClass(object1, object2, breadCrumb);
            }
            else if (IsTimespan(t1))
            {
                CompareTimespan(object1, object2, breadCrumb);
            }
            else if (IsStruct(t1))
            {
                CompareStruct(object1, object2, breadCrumb);
            }
            else
            {
                throw new NotImplementedException("Cannot compare object of type " + t1.Name);
            }

        }



        /// <summary>
        /// Check if any type has attributes that should be bypassed
        /// </summary>
        /// <returns></returns>
        protected bool IgnoredByAttribute(Type type)
        {
            foreach (Type attributeType in AttributesToIgnore)
            {
                if (type.GetCustomAttributes(attributeType, false).Length > 0)
                    return true;
            }

            return false;
        }

        protected void CompareDataRow(object object1, object object2, string breadCrumb)
        {
            DataRow dataRow1 = object1 as DataRow;
            DataRow dataRow2 = object2 as DataRow;

            if (dataRow1 == null) //This should never happen, null check happens one level up
                throw new ArgumentNullException("object1");

            if (dataRow2 == null) //This should never happen, null check happens one level up
                throw new ArgumentNullException("object2");

            for (int i = 0; i < dataRow1.Table.Columns.Count; i++)
            {
                //If we should ignore it, skip it
                if (ElementsToIgnore.Contains(dataRow1.Table.Columns[i].ColumnName))
                    continue;

                //If we should ignore read only, skip it
                if (!CompareReadOnly && dataRow1.Table.Columns[i].ReadOnly)
                    continue;

                //Both are null
                if (dataRow1.IsNull(i) && dataRow2.IsNull(i))
                    continue;

                string currentBreadCrumb = AddBreadCrumb(breadCrumb, string.Empty, string.Empty, dataRow1.Table.Columns[i].ColumnName);

                //Check if one of them is null
                if (dataRow1.IsNull(i))
                {
                    Differences.Add(string.Format("object1{0} == null && object2{0} != null ((null),{1})", currentBreadCrumb, cStr(object2)));
                    return;
                }

                if (dataRow2.IsNull(i))
                {
                    Differences.Add(string.Format("object1{0} != null && object2{0} == null ({1},(null))", currentBreadCrumb, cStr(object1)));
                    return;
                }

                Compare(dataRow1[i], dataRow2[i], currentBreadCrumb);

                if (Differences.Count >= MaxDifferences)
                    return;
            }
        }

        protected void CompareDataTable(object object1, object object2, string breadCrumb)
        {
            DataTable dataTable1 = object1 as DataTable;
            DataTable dataTable2 = object2 as DataTable;

            if (dataTable1 == null) //This should never happen, null check happens one level up
                throw new ArgumentNullException("object1");

            if (dataTable2 == null) //This should never happen, null check happens one level up
                throw new ArgumentNullException("object2");

            //If we should ignore it, skip it
            if (ElementsToIgnore.Contains(dataTable1.TableName))
                return;

            //There must be the same amount of rows in the datatable
            if (dataTable1.Rows.Count != dataTable2.Rows.Count)
            {
                Differences.Add(string.Format("object1{0}.Rows.Count != object2{0}.Rows.Count ({1},{2})", breadCrumb,
                                              dataTable1.Rows.Count, dataTable2.Rows.Count));

                if (Differences.Count >= MaxDifferences)
                    return;
            }

            //There must be the same amount of columns in the datatable
            if (dataTable1.Columns.Count != dataTable2.Columns.Count)
            {
                Differences.Add(string.Format("object1{0}.Columns.Count != object2{0}.Columns.Count ({1},{2})", breadCrumb,
                                              dataTable1.Columns.Count, dataTable2.Columns.Count));

                if (Differences.Count >= MaxDifferences)
                    return;
            }

            for (int i = 0; i < dataTable1.Rows.Count; i++)
            {
                string currentBreadCrumb = AddBreadCrumb(breadCrumb, "Rows", string.Empty, i);

                CompareDataRow(dataTable1.Rows[i], dataTable2.Rows[i], currentBreadCrumb);

                if (Differences.Count >= MaxDifferences)
                    return;
            }
        }

        protected void CompareDataset(object object1, object object2, string breadCrumb)
        {
            DataSet dataSet1 = object1 as DataSet;
            DataSet dataSet2 = object2 as DataSet;

            if (dataSet1 == null) //This should never happen, null check happens one level up
                throw new ArgumentNullException("object1");

            if (dataSet2 == null) //This should never happen, null check happens one level up
                throw new ArgumentNullException("object2");


            //There must be the same amount of tables in the dataset
            if (dataSet1.Tables.Count != dataSet2.Tables.Count)
            {
                Differences.Add(string.Format("object1{0}.Tables.Count != object2{0}.Tables.Count ({1},{2})", breadCrumb,
                                              dataSet1.Tables.Count, dataSet2.Tables.Count));

                if (Differences.Count >= MaxDifferences)
                    return;
            }

            for (int i = 0; i < dataSet1.Tables.Count; i++)
            {
                string currentBreadCrumb = AddBreadCrumb(breadCrumb, "Tables", string.Empty, dataSet1.Tables[i].TableName);

                CompareDataTable(dataSet1.Tables[i], dataSet2.Tables[i], currentBreadCrumb);

                if (Differences.Count >= MaxDifferences)
                    return;
            }
        }

        protected bool IsTimespan(Type t)
        {
            return t == typeof(TimeSpan);
        }

        protected bool IsPointer(Type t)
        {
            return t == typeof(IntPtr) || t == typeof(UIntPtr);
        }

        protected bool IsEnum(Type t)
        {
            return t.IsEnum;
        }

        protected bool IsStruct(Type t)
        {
            return t.IsValueType && !IsSimpleType(t);
        }

        protected bool IsSimpleType(Type t)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                t = Nullable.GetUnderlyingType(t);
            }

            return t.IsPrimitive
                || t == typeof(DateTime)
                || t == typeof(decimal)
                || t == typeof(string)
                || t == typeof(Guid);

        }

        protected bool ValidStructSubType(Type t)
        {
            return IsSimpleType(t)
                || IsEnum(t)
                || IsArray(t)
                || IsClass(t)
                || IsIDictionary(t)
                || IsTimespan(t)
                || IsIList(t);
        }

        protected bool IsArray(Type t)
        {
            return t.IsArray;
        }

        protected bool IsClass(Type t)
        {
            return t.IsClass;
        }

        protected bool IsIDictionary(Type t)
        {
            return t.GetInterface("System.Collections.IDictionary", true) != null;
        }

        protected bool IsDataset(Type t)
        {
            return t == typeof(DataSet);
        }

        protected bool IsDataRow(Type t)
        {
            return t == typeof(DataRow);
        }

        protected bool IsDataTable(Type t)
        {
            return t == typeof(DataTable);
        }

        protected bool IsIList(Type t)
        {
            return t.GetInterface("System.Collections.IList", true) != null;
        }

        protected bool IsChildType(Type t)
        {
            return !IsSimpleType(t)
                && (IsClass(t)
                    || IsArray(t)
                    || IsIDictionary(t)
                    || IsIList(t)
                    || IsStruct(t));
        }

        /// <summary>
        /// Compare a timespan struct
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="breadCrumb"></param>
        protected void CompareTimespan(object object1, object object2, string breadCrumb)
        {
            if (((TimeSpan)object1).Ticks != ((TimeSpan)object2).Ticks)
            {
                Differences.Add(string.Format("object1{0}.Ticks != object2{0}.Ticks", breadCrumb));
            }
        }

        /// <summary>
        /// Compare a pointer struct
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="breadCrumb"></param>
        protected void ComparePointer(object object1, object object2, string breadCrumb)
        {
            if (
                (object1.GetType() == typeof(IntPtr) && object2.GetType() == typeof(IntPtr) && ((IntPtr)object1) != ((IntPtr)object2)) ||
                (object1.GetType() == typeof(UIntPtr) && object2.GetType() == typeof(UIntPtr) && ((UIntPtr)object1) != ((UIntPtr)object2))
                )
            {
                Differences.Add(string.Format("object1{0} != object2{0}", breadCrumb));
            }
        }

        /// <summary>
        /// Compare an enumeration
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="breadCrumb"></param>
        protected void CompareEnum(object object1, object object2, string breadCrumb)
        {
            if (object1.ToString() != object2.ToString())
            {
                string currentBreadCrumb = AddBreadCrumb(breadCrumb, object1.GetType().Name, string.Empty, -1);
                Differences.Add(string.Format("object1{0} != object2{0} ({1},{2})", currentBreadCrumb, object1, object2));
            }
        }

        /// <summary>
        /// Compare a simple type
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="breadCrumb"></param>
        protected void CompareSimpleType(object object1, object object2, string breadCrumb)
        {
            if (object2 == null) //This should never happen, null check happens one level up
                throw new ArgumentNullException("object2");

            IComparable valOne = object1 as IComparable;

            if (valOne == null) //This should never happen, null check happens one level up
                throw new ArgumentNullException("object1");

            if (valOne.CompareTo(object2) != 0)
            {
                Differences.Add(string.Format("object1{0} != object2{0} ({1},{2})", breadCrumb, object1, object2));
            }
        }



        /// <summary>
        /// Compare a struct
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="breadCrumb"></param>
        protected void CompareStruct(object object1, object object2, string breadCrumb)
        {
            try
            {
                _parents.Add(object1);
                _parents.Add(object2);

                Type t1 = object1.GetType();

                PerformCompareFields(t1, object1, object2, true, breadCrumb);
                PerformCompareProperties(t1, object1, object2, true, breadCrumb);
            }
            finally
            {
                _parents.Remove(object1);
                _parents.Remove(object2);
            }
        }

        /// <summary>
        /// Compare the properties, fields of a class
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="breadCrumb"></param>
        protected void CompareClass(object object1, object object2, string breadCrumb)
        {
            try
            {
                _parents.Add(object1);
                _parents.Add(object2);

                Type t1 = object1.GetType();

                //We ignore the class name
                if (ElementsToIgnore.Contains(t1.Name) || IgnoredByAttribute(t1))
                    return;

                //Compare the properties
                if (CompareProperties)
                    PerformCompareProperties(t1, object1, object2, false, breadCrumb);

                //Compare the fields
                if (CompareFields)
                    PerformCompareFields(t1, object1, object2, false, breadCrumb);
            }
            finally
            {
                _parents.Remove(object1);
                _parents.Remove(object2);
            }
        }


        /// <summary>
        /// Compare the fields of a class
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="breadCrumb"></param>
        protected void PerformCompareFields(Type t1,
            object object1,
            object object2,
            bool structCompare,
            string breadCrumb)
        {
            IEnumerable<FieldInfo> currentFields = GetFieldInfo(t1);

            foreach (FieldInfo item in currentFields)
            {
                //Ignore invalid struct fields
                if (structCompare && !ValidStructSubType(item.FieldType))
                    continue;

                //Skip if this is a shallow compare
                if (!CompareChildren && IsChildType(item.FieldType))
                    continue;

                //If we should ignore it, skip it
                if (ElementsToIgnore.Contains(item.Name) || IgnoredByAttribute(item.FieldType))
                    continue;

                object objectValue1 = item.GetValue(object1);
                object objectValue2 = item.GetValue(object2);

                bool object1IsParent = objectValue1 != null && (objectValue1 == object1 || _parents.Contains(objectValue1));
                bool object2IsParent = objectValue2 != null && (objectValue2 == object2 || _parents.Contains(objectValue2));

                //Skip fields that point to the parent
                if (IsClass(item.FieldType)
                    && (object1IsParent || object2IsParent))
                {
                    continue;
                }

                string currentCrumb = AddBreadCrumb(breadCrumb, item.Name, string.Empty, -1);

                Compare(objectValue1, objectValue2, currentCrumb);

                if (Differences.Count >= MaxDifferences)
                    return;
            }
        }

        protected IEnumerable<FieldInfo> GetFieldInfo(Type type)
        {
            if (Caching && _fieldCache.ContainsKey(type))
                return _fieldCache[type];

            FieldInfo[] currentFields;

            if (CompareprotectedFields && !CompareStaticFields)
                currentFields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            else if (CompareprotectedFields && CompareStaticFields)
                currentFields = type.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            else
                currentFields = type.GetFields(); //Default is public instance and static

            if (Caching)
                _fieldCache.Add(type, currentFields);

            return currentFields;
        }


        /// <summary>
        /// Compare the properties of a class
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="breadCrumb"></param>
        protected void PerformCompareProperties(Type t1,
            object object1,
            object object2,
            bool structCompare,
            string breadCrumb)
        {
            IEnumerable<PropertyInfo> currentProperties = GetPropertyInfo(t1);

            foreach (PropertyInfo info in currentProperties)
            {
                //Ignore invalid struct fields
                if (structCompare && !ValidStructSubType(info.PropertyType))
                    continue;

                //If we can't read it, skip it
                if (info.CanRead == false)
                    continue;

                //Skip if this is a shallow compare
                if (!CompareChildren && IsChildType(info.PropertyType))
                    continue;

                //If we should ignore it, skip it
                if (ElementsToIgnore.Contains(info.Name) || IgnoredByAttribute(info.PropertyType))
                    continue;

                //If we should ignore read only, skip it
                if (!CompareReadOnly && info.CanWrite == false)
                    continue;

                object objectValue1;
                object objectValue2;
                if (!IsValidIndexer(info, breadCrumb))
                {
                    objectValue1 = info.GetValue(object1, null);
                    objectValue2 = info.GetValue(object2, null);
                }
                else
                {
                    CompareIndexer(info, object1, object2, breadCrumb);
                    continue;
                }

                bool object1IsParent = objectValue1 != null && (objectValue1 == object1 || _parents.Contains(objectValue1));
                bool object2IsParent = objectValue2 != null && (objectValue2 == object2 || _parents.Contains(objectValue2));

                //Skip properties where both point to the corresponding parent
                if ((IsClass(info.PropertyType) || IsStruct(info.PropertyType)) && (object1IsParent && object2IsParent))
                {
                    continue;
                }

                string currentCrumb = AddBreadCrumb(breadCrumb, info.Name, string.Empty, -1);

                Compare(objectValue1, objectValue2, currentCrumb);

                if (Differences.Count >= MaxDifferences)
                    return;
            }
        }

        protected IEnumerable<PropertyInfo> GetPropertyInfo(Type type)
        {
            if (Caching && _propertyCache.ContainsKey(type))
                return _propertyCache[type];

            PropertyInfo[] currentProperties;

            if (CompareprotectedProperties && !CompareStaticProperties)
                currentProperties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            else if (CompareprotectedProperties && CompareStaticProperties)
                currentProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            else if (!CompareStaticProperties)
                currentProperties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            else
                currentProperties = type.GetProperties(); //Default is public instance and static

            if (Caching)
                _propertyCache.Add(type, currentProperties);

            return currentProperties;
        }

        protected bool IsValidIndexer(PropertyInfo info, string breadCrumb)
        {
            ParameterInfo[] indexers = info.GetIndexParameters();

            if (indexers.Length == 0)
            {
                return false;
            }

            if (indexers.Length > 1)
            {
                throw new Exception("Cannot compare objects with more than one indexer for object " + breadCrumb);
            }

            if (indexers[0].ParameterType != typeof(Int32))
            {
                throw new Exception("Cannot compare objects with a non integer indexer for object " + breadCrumb);
            }

            if (info.ReflectedType.GetProperty("Count") == null)
            {
                throw new Exception("Indexer must have a corresponding Count property for object " + breadCrumb);
            }

            if (info.ReflectedType.GetProperty("Count").PropertyType != typeof(Int32))
            {
                throw new Exception("Indexer must have a corresponding Count property that is an integer for object " + breadCrumb);
            }

            return true;
        }
        protected void CompareIndexer(PropertyInfo info, object object1, object object2, string breadCrumb)
        {
            string currentCrumb;
            int indexerCount1 = (int)info.ReflectedType.GetProperty("Count").GetGetMethod().Invoke(object1, new object[] { });
            int indexerCount2 = (int)info.ReflectedType.GetProperty("Count").GetGetMethod().Invoke(object2, new object[] { });

            //Indexers must be the same length
            if (indexerCount1 != indexerCount2)
            {
                currentCrumb = AddBreadCrumb(breadCrumb, info.Name, string.Empty, -1);
                Differences.Add(string.Format("object1{0}.Count != object2{0}.Count ({1},{2})", currentCrumb,
                                              indexerCount1, indexerCount2));

                if (Differences.Count >= MaxDifferences)
                    return;
            }

            // Run on indexer
            for (int i = 0; i < indexerCount1; i++)
            {
                currentCrumb = AddBreadCrumb(breadCrumb, info.Name, string.Empty, i);
                object objectValue1 = info.GetValue(object1, new object[] { i });
                object objectValue2 = info.GetValue(object2, new object[] { i });
                Compare(objectValue1, objectValue2, currentCrumb);

                if (Differences.Count >= MaxDifferences)
                    return;
            }
        }

        /// <summary>
        /// Compare a dictionary
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="breadCrumb"></param>
        protected void CompareIDictionary(object object1, object object2, string breadCrumb)
        {
            IDictionary iDict1 = object1 as IDictionary;
            IDictionary iDict2 = object2 as IDictionary;

            if (iDict1 == null) //This should never happen, null check happens one level up
                throw new ArgumentNullException("object1");

            if (iDict2 == null) //This should never happen, null check happens one level up
                throw new ArgumentNullException("object2");

            try
            {
                _parents.Add(object1);
                _parents.Add(object2);

                //Objects must be the same length
                if (iDict1.Count != iDict2.Count)
                {
                    Differences.Add(string.Format("object1{0}.Count != object2{0}.Count ({1},{2})", breadCrumb,
                                                  iDict1.Count, iDict2.Count));

                    if (Differences.Count >= MaxDifferences)
                        return;
                }

                IDictionaryEnumerator enumerator1 = iDict1.GetEnumerator();
                IDictionaryEnumerator enumerator2 = iDict2.GetEnumerator();

                while (enumerator1.MoveNext() && enumerator2.MoveNext())
                {
                    string currentBreadCrumb = AddBreadCrumb(breadCrumb, "Key", string.Empty, -1);

                    Compare(enumerator1.Key, enumerator2.Key, currentBreadCrumb);

                    if (Differences.Count >= MaxDifferences)
                        return;

                    currentBreadCrumb = AddBreadCrumb(breadCrumb, "Value", string.Empty, -1);

                    Compare(enumerator1.Value, enumerator2.Value, currentBreadCrumb);

                    if (Differences.Count >= MaxDifferences)
                        return;
                }
            }
            finally
            {
                _parents.Remove(object1);
                _parents.Remove(object2);
            }
        }

        /// <summary>
        /// Convert an object to a nicely formatted string
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected string cStr(object obj)
        {
            try
            {
                if (obj == null)
                    return "(null)";

                if (obj == DBNull.Value)
                    return "System.DBNull.Value";

                return obj.ToString();
            }
            catch
            {
                return string.Empty;
            }
        }


        /// <summary>
        /// Compare an array or something that implements IList
        /// </summary>
        /// <param name="object1"></param>
        /// <param name="object2"></param>
        /// <param name="breadCrumb"></param>
        protected void CompareIList(object object1, object object2, string breadCrumb)
        {
            IList ilist1 = object1 as IList;
            IList ilist2 = object2 as IList;

            if (ilist1 == null) //This should never happen, null check happens one level up
                throw new ArgumentNullException("object1");

            if (ilist2 == null) //This should never happen, null check happens one level up
                throw new ArgumentNullException("object2");

            try
            {
                _parents.Add(object1);
                _parents.Add(object2);

                //Objects must be the same length
                if (ilist1.Count != ilist2.Count)
                {
                    Differences.Add(string.Format("object1{0}.Count != object2{0}.Count ({1},{2})", breadCrumb,
                                                  ilist1.Count, ilist2.Count));

                    if (Differences.Count >= MaxDifferences)
                        return;
                }

                IEnumerator enumerator1 = ilist1.GetEnumerator();
                IEnumerator enumerator2 = ilist2.GetEnumerator();
                int count = 0;

                while (enumerator1.MoveNext() && enumerator2.MoveNext())
                {
                    string currentBreadCrumb = AddBreadCrumb(breadCrumb, string.Empty, string.Empty, count);

                    Compare(enumerator1.Current, enumerator2.Current, currentBreadCrumb);

                    if (Differences.Count >= MaxDifferences)
                        return;

                    count++;
                }
            }
            finally
            {
                _parents.Remove(object1);
                _parents.Remove(object2);
            }
        }



        /// <summary>
        /// Add a breadcrumb to an existing breadcrumb
        /// </summary>
        /// <param name="existing"></param>
        /// <param name="name"></param>
        /// <param name="extra"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected string AddBreadCrumb(string existing, string name, string extra, string index)
        {
            bool useIndex = !String.IsNullOrEmpty(index);
            bool useName = name.Length > 0;
            StringBuilder sb = new StringBuilder();

            sb.Append(existing);

            if (useName)
            {
                sb.AppendFormat(".");
                sb.Append(name);
            }

            sb.Append(extra);

            if (useIndex)
            {
                int result = -1;
                sb.AppendFormat(Int32.TryParse(index, out result) ? "[{0}]" : "[\"{0}\"]", index);
            }

#if BREADCRUMB
            Console.WriteLine(sb.ToString());
#endif

            return sb.ToString();
        }

        /// <summary>
        /// Add a breadcrumb to an existing breadcrumb
        /// </summary>
        /// <param name="existing"></param>
        /// <param name="name"></param>
        /// <param name="extra"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected string AddBreadCrumb(string existing, string name, string extra, int index)
        {
            return AddBreadCrumb(existing, name, extra, index >= 0 ? index.ToString() : null);
        }

        #endregion

    }
}
