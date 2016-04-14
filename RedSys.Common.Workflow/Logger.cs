using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.SharePoint;

namespace RedSys.Common.Workflow
{
    public class Logger
    {
        const string connectionString = "Data Source=SRV-NSOL-KLM;Initial Catalog=WSS_Content_EDF;User Id=xmlexchange;Password=Aa1234;";
        public static void AddAloneEntry(SPListItem CurItem, SPUser User, string RecordType, string RecordStage, string result, string comment)
        {
            string status = CurItem["Статус документа"] == null ? "" : CurItem["Статус документа"].ToString();
            AddRecord(CurItem.ParentList.Title, CurItem["Штрих-код"].ToString(), CurItem["Штрих-код"].ToString(), DateTime.Now, false, RecordType, RecordStage,
                User.LoginName, User.Name, User.Email, status, result, "", "", DateTime.Now, comment, CurItem.ID.ToString());
        }

        public static void AddParentEntry(SPListItem CurItem, SPUser User, string RecordType, string RecordStage, string result, string UpdateFields, string FieldValues, string comment, string PacageCode, string status)
        {
            AddRecord(CurItem.ParentList.Title, PacageCode, CurItem["Штрих-код"].ToString(), DateTime.Now, true, RecordType, RecordStage,
                User.LoginName, User.Name, User.Email, status, result, UpdateFields, FieldValues, DateTime.Now, comment, CurItem.ID.ToString());
        }

        protected static void AddRecord(string LibName, string PacageCode, string Barcode, DateTime CreationTime,
            bool NeedChildUpdate, string RecordType, string RecordStage, string Login, string Name,
            string Role, string DocStatus, string result, string UpdateFields, string FieldValues, DateTime AssignDate, string comment, string ItemID)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string NeedUpdate = NeedChildUpdate ? "1" : "0";
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("INSERT INTO dbo.EnsolLog VALUES (N'{0}',N'{1}',N'{2}',N'{3}',{4},N'{5}',N'{6}',N'{7}',N'{8}',N'{9}',N'{10}',N'{11}',N'{12}',N'{13}',N'{14}',N'{15}',N'{16}',N'{17}');",
                    PacageCode, Barcode, CreationTime.ToString(), CreationTime.ToString(), NeedUpdate,
                    RecordType, RecordStage, Login, Name, Role, DocStatus, result, UpdateFields, FieldValues, AssignDate.ToString(), comment, LibName, ItemID);
                string QueryString = sb.ToString();
                SqlCommand command = new SqlCommand(QueryString, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public static List<LogData> FindNotUpdated()
        {
            List<LogData> barcodes = new List<LogData>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string QueryString = "Select ID, [ID документа],[Библиотека документа], [Поля для обновления],[Значения полей], [ItemID] from dbo.EnsolLog " +
                    " WHERE Обновить=1";
                SqlCommand command = new SqlCommand(QueryString, connection);
                connection.Open();
                using (SqlDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        LogData ld = new LogData();
                        ld.ID = int.Parse(dr[0].ToString());
                        ld.Barcode = dr[1].ToString();
                        ld.LibName = dr[2].ToString();
                        ld.Fields = dr[3].ToString();
                        ld.Values = dr[4].ToString();
                        ld.ItemID = int.Parse(dr[5].ToString());
                        barcodes.Add(ld);
                    }
                }
            }
            return barcodes;
        }

        public static void SetUpdateFlag(LogData data)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string QueryString = "UPDATE dbo.EnsolLog SET Обновить=0,[Дата обновления]= '" + DateTime.Now.ToString() + "' WHERE ID=" + data.ID + ";";
                SqlCommand command = new SqlCommand(QueryString, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }

    public class LogData
    {
        public string LibName { get; set; }
        public string Barcode { get; set; }
        public string Fields { get; set; }
        public string Values { get; set; }
        public int ItemID { get; set; }
        public int ID { get; set; }
    }

}
