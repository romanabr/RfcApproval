using System;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using Microsoft.SharePoint;
using RedSys.RFC.Core.Helper;
using RedSys.Common.Workflow;

namespace WorkflowListsTemplates.Features.Feature1
{
    [Guid("d8f2e059-1b27-4e80-9df2-20c0785d7838")]
    public class Feature1EventReceiver : SPFeatureReceiver
    {
        public override void FeatureActivated(SPFeatureReceiverProperties properties)
        {
            SPWeb spWeb = properties.Feature.Parent as SPWeb;
            spWeb.AllowUnsafeUpdates = true;
            try
            {
                CreateList(spWeb, "Руководители ответственных", new string[] { "Ответственный", "Руководитель ответственного" });
                CreateList(spWeb, "Заместители", new string[] { "Заместитель", "Замещаемый", "Начальная дата", "Конечная дата", "Описание" });

                CreateList(spWeb, "Выходные", new string[] { "Дата выходного" });

                CreateList(spWeb, "Типы задач", new string[] { });

                CreateList(spWeb, "Статистика задач", new string[] { "Исполнители задачи", "Дата начала",
                    "Срок исполнения", "Дата завершения", "Документ", "Документ ИД", "Библиотека",
                    "Завершивший исполнитель", "Решение согласующего" });
                SPList spList = null;
                SPField spField = null;
                string fieldXml = "";
                string temp = "";
                CreateList(spWeb, "Список ролей", new string[] { "Вид роли" });
                try
                {
                    spList = spWeb.Lists["Список ролей"];
                    spField = spWeb.Fields[Constant.Role_Ref];
                    fieldXml = spField.SchemaXml;
                    temp = fieldXml.Substring(fieldXml.IndexOf("List=")).Split(new char[1] { '\"' })[1];
                    fieldXml = fieldXml.Replace(temp, spList.ID.ToString("B").ToLower());
                    spField.SchemaXml = fieldXml;
                    spField.Update();
                }
                catch { }

                SPList NewList = CreateList(spWeb, "Пользователи и роли", new string[] { "Учетная запись. Справочник AD", "Исполнитель по умолчанию" });
                try
                {
                    NewList.Fields.Add(spField);
                }
                catch { }
                NewList.Update();

                NewList = CreateList(spWeb, "Список этапов", new string[] { "Номер этапа", "Тип этапа", "Ожидание", "Тип документа", "Название этапа", "Длительность",
               "Пропускать без исполнителя","Пропускать права", "Принудительное согласование","Тип согласования", "№ следующего этапа положительный","№ следующего этапа отрицательный",
               "Вносить в лист согласования","Создавать задачу", "Содержание задания", "Поле карточки для сравнения", "Значение для сравнения", "Тип поля", "Правило", "Поля для переноса в задачу",
               "Текст уведомления","Исключать повтор согласующего", "Уведомление по завершению задачи", "Уведомление по завершению этапа", "Уведомление по завершению процесса", "Права на карточку","Повторное согласование"});
                try
                {
                    NewList.Fields.Add(spField);

                    spList = spWeb.Lists["Типы задач"];
                    spField = spWeb.Fields["Тип задания"];
                    fieldXml = spField.SchemaXml;
                    temp = fieldXml.Substring(fieldXml.IndexOf("List=")).Split(new char[1] { '\"' })[1];
                    fieldXml = fieldXml.Replace(temp, spList.ID.ToString("B").ToLower());
                    spField.SchemaXml = fieldXml;
                    spField.Update();
                    NewList.Fields.Add(spField);

                    NewList.Update();
                }
                catch { }
            }
            catch (Exception ex)
            {
               ExceptionHelper.DUmpException(ex);
            }
            finally
            {
            }
        }

        public SPList CreateList(SPWeb web, string Name, string[] fields)
        {
            SPList ol = web.Lists.TryGetList(Name);
            if (ol == null)
            {
                web.AllowUnsafeUpdates = true;
                web.Lists.Add(Name, Name, SPListTemplateType.GenericList);
                ol = web.Lists[Name];
                foreach (string s in fields)
                {
                    SPField fld = web.Fields[s];
                    ol.Fields.Add(fld);
                }
                ol.Update();
            }
            return ol;
        }
    }
}
