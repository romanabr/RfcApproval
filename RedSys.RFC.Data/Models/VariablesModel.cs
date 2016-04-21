using Microsoft.SharePoint;
using RedSys.RFC.Core.Helper;
using RedSys.RFC.Data.Fields;
using RedSys.RFC.Data.Lists;
using SPMeta2.Definitions;
using SPMeta2.Definitions.Fields;
using SPMeta2.Enumerations;
using SPMeta2.Syntax.Default;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSys.RFC.Data.Models
{
    public class VariablesModel : SPModel
    {
        public VariablesModel(SPWeb web) : base(web)
        {

        }
        

        public override void Deploy()
        {
            base.Deploy();
            var model = SPMeta2Model.NewSiteModel(site =>
           {
               site.AddField(WorkflowFields.WorkflowValue);
           });


            var webmodel = SPMeta2Model.NewWebModel(web =>
        {
            web.AddList(RFCLists.VariablesList, list =>
             {
                 list.AddField(WorkflowFields.WorkflowValue);

                 list.AddListItems(new List<ListItemDefinition>
                            {
                            new ListItemDefinition
                            {
                                 Title = "PSE.CustomUpload - Поля",
                                 SystemUpdate= true,
                                  Overwrite = false,
                                   Values = new List<FieldValue>
                                   {
                                        new FieldValue
                                        {
                                             FieldName = WorkflowFields.WorkflowValue.InternalName,
                                             Value = "Краткое описание"
                                        }
                                   }
                            },
                            new ListItemDefinition
                            {

                                 Title = "PSE.CustomUpload - Список",
                                 SystemUpdate= true,
                                  Overwrite = false,
                                   Values = new List<FieldValue>
                                   {
                                        new FieldValue
                                        {
                                             FieldName = WorkflowFields.WorkflowValue.InternalName,
                                             Value = "Управление изменениями"
                                        }
                                   }
                            }
                            ,
                            new ListItemDefinition
                            {

                                Title = "PSE.CustomUpload - CheckIn",
                                SystemUpdate = true,
                                Overwrite = false,
                                Values = new List<FieldValue>
                                   {
                                        new FieldValue
                                        {
                                             FieldName = WorkflowFields.WorkflowValue.InternalName,
                                             Value = "true"
                                        }
                                   }
                            } ,
                            new ListItemDefinition
                            {

                                Title = "PSE.CustomUpload - Version",
                                SystemUpdate = true,
                                Overwrite = false,
                                Values = new List<FieldValue>
                                   {
                                        new FieldValue
                                        {
                                             FieldName = WorkflowFields.WorkflowValue.InternalName,
                                             Value = "false"
                                        }
                                   }
                            }
                 });
             });
        });
            DeployModel(model);
            DeployModel(webmodel);
        }
    }
}
