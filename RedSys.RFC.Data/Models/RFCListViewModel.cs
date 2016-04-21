using Microsoft.SharePoint;
using RedSys.RFC.Core.Helper;
using RedSys.RFC.Data.ContentTypes;
using RedSys.RFC.Data.Lists;
using SPMeta2.Definitions;
using SPMeta2.Syntax.Default;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSys.RFC.Data.Models
{
   public  class RFCListViewModel :SPModel
    {
        public RFCListViewModel(SPWeb web) : base(web)
        {

        }

        public override void Deploy()
        {
            var listItemDefinitions = GetListItemDefinitions();
            var typeDefinitions = GetTypeDefinitions(listItemDefinitions);
            var managerItemDefinitions = GetManagerItemDefinition();
            var groupItemDefinitions = GetGroupItemDefinition();
            var responsibleItemDefinitions = GetResponsibleItemDefinitions();
            var catalogueItemDefinitions = GetCatalogueItemDefinitions();
            //var effectItemDefinitions = GetEffectItemDefinitions();

            var model = SPMeta2Model.NewWebModel(web =>
            {
                web.AddList(RFCLists.RfcCategoryList, list =>
                {list.AddListItems(listItemDefinitions);
                list.AddListView(RFCViews.RfcCategoryListView);
            });


                web.AddList(RFCLists.KETypeList, list =>
                {list.AddListItems(typeDefinitions);
                list.AddListView(RFCViews.RfcTypeListView);
            });



                web.AddList(RFCLists.RFCManagerList, list =>
                {
                 list.AddListItems(managerItemDefinitions);
                list.AddListView(RFCViews.RFCManagerListView);
            });

                web.AddList(RFCLists.RfcGroupKe, list =>
                {list.AddListItems(groupItemDefinitions);
                });

                web.AddList(RFCLists.KEResponsibleList, list =>
                {list.AddListItems(responsibleItemDefinitions);
            });

                web.AddList(RFCLists.KECatalogueList, list =>
                {
                    //list.AddListItems(catalogueItemDefinitions);
                list.AddListView(RFCViews.RfcKeCatalogueListView);
            });

                web.AddList(RFCLists.KeEffectList, list =>
                {
                    //list.AddListItems(effectItemDefinitions);
                list.AddListView(RFCViews.RFCKEEffectListView);
            });

                web.AddList(RFCLists.RfcKeList, list =>
                {list.AddListView(RFCViews.RfcKeListView);
            });

                web.AddList(RFCLists.RfcUserList, list =>
                {list.AddListView(RFCViews.RfcUserListView);
            });

                web.AddList(RFCLists.KeApproveTaskList, list =>
                {list.AddListView(RFCViews.RfcKeApproveTaskView);

            });

               
            });

            DeployModel(model);

        }

        private List<ListItemDefinition> GetEffectItemDefinitions()
        {
            return new List<ListItemDefinition>
            {
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.KEEffect.Name,
                    Title = "1-2", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
                    Values = new List<FieldValue>
                    {
                         new FieldValue { FieldName = RFCFields.KeParentLink.InternalName, Value = "1;#База данных управления конфигурациями" },
                         new FieldValue { FieldName = RFCFields.KeChildLink.InternalName, Value = "2;#Управление жизненным циклом приложений" }
                    }
                },
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.KEEffect.Name,
                    Title = "2-3", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
                    Values = new List<FieldValue>
                    {
                         new FieldValue { FieldName = RFCFields.KeParentLink.InternalName, Value = "2;#Управление жизненным циклом приложений" },
                         new FieldValue { FieldName = RFCFields.KeChildLink.InternalName, Value = "3;#Согласование по IT" }
                    }
                },
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.KEEffect.Name,
                    Title = "1-4", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
                    Values = new List<FieldValue>
                    {
                         new FieldValue { FieldName = RFCFields.KeParentLink.InternalName, Value = "1;#База данных управления конфигурациями" },
                         new FieldValue { FieldName = RFCFields.KeChildLink.InternalName, Value = "4;#Резервация ресурсов" }
                    }
                }
            };
        }

        private List<ListItemDefinition> GetCatalogueItemDefinitions()
        {
            return new List<ListItemDefinition>
            {
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.KECatalogue.Name,
                    Title = "База данных управления конфигурациями", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
                    Values = new List<FieldValue>
                    {
                         new FieldValue { FieldName = RFCFields.KeMnemonica.InternalName, Value = "CMDB" },
                         new FieldValue { FieldName = RFCFields.KeToTypeLink.InternalName, Value = "1;#База данных" }
                    }
                },
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.KECatalogue.Name,
                    Title = "Управление жизненным циклом приложений", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
                    Values = new List<FieldValue>
                    {
                         new FieldValue { FieldName = RFCFields.KeMnemonica.InternalName, Value = "ALMI" },
                         new FieldValue { FieldName = RFCFields.KeToTypeLink.InternalName, Value = "2;Приложение" }
                    }
                },
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.KECatalogue.Name,
                    Title = "Согласование по IT", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
                    Values = new List<FieldValue>
                    {
                         new FieldValue { FieldName = RFCFields.KeMnemonica.InternalName, Value = "SPAPPROVALS" },
                         new FieldValue { FieldName = RFCFields.KeToTypeLink.InternalName, Value = "2;Приложение" }
                    }
                },
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.KECatalogue.Name,
                    Title = "Резервация ресурсов", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
                    Values = new List<FieldValue>
                    {
                         new FieldValue { FieldName = RFCFields.KeMnemonica.InternalName, Value = "RESOURCERESERVATION" },
                         new FieldValue { FieldName = RFCFields.KeToTypeLink.InternalName, Value = "2;Приложение" }
                    }
                }
            };
        }

        private List<ListItemDefinition> GetResponsibleItemDefinitions()
        {
            return new List<ListItemDefinition>
            {
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.KEResponsible.Name,
                    Title = "ALMI", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
                    Values = new List<FieldValue>
                    {
                         new FieldValue { FieldName = RFCFields.KeToKeLink.InternalName, Value = "2;#Управление жизненным циклом приложений" }
                    }
                },
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.KEResponsible.Name,
                    Title = "CMDB", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
                    Values = new List<FieldValue>
                    {
                         new FieldValue { FieldName = RFCFields.KeToKeLink.InternalName, Value = "1;#База данных управления конфигурациями" }
                    }
                },
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.KEResponsible.Name,
                    Title = "SPAPPROVALS", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
                    Values = new List<FieldValue>
                    {
                         new FieldValue { FieldName = RFCFields.KeToKeLink.InternalName, Value = "3;#Согласование по IT" }
                    }
                },
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.KEResponsible.Name,
                    Title = "SPAPPROVALS1", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
                    Values = new List<FieldValue>
                    {
                         new FieldValue { FieldName = RFCFields.KeToKeLink.InternalName, Value = "3;#Согласование по IT" }
                    }
                }
            };
        }

        private List<ListItemDefinition> GetGroupItemDefinition()
        {
            return new List<ListItemDefinition>
            {
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.KEGroup.Name,
                    Title = "База данных", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,

                },
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.KEGroup.Name,
                    Title = "Приложение", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,

                }
            };
        }

        private List<ListItemDefinition> GetManagerItemDefinition()
        {
            SPUser user = currentWeb.SiteUsers[1];
            return new List<ListItemDefinition>
            {
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.RfcManager.Name,
                    Title = "Менеджер 1", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
                    Values = new   List<FieldValue>
                    {
                        new FieldValue
                        {
                            FieldName = RFCFields.Type.InternalName,
                            Value = "1;#Обычное"
                        },
                        new FieldValue
                        {
                            FieldName = RFCFields.Manager.InternalName,
                            Value =  new SPFieldUserValue(currentWeb,user.ID,user.LoginName)
                        }
                    }
                },
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.RfcManager.Name,
                    Title = "Менеджер 2", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
                    Values = new   List<FieldValue>
                    {
                        new FieldValue
                        {
                            FieldName = RFCFields.Type.InternalName,
                            Value = "2;#Аварийное"
                        },
                        new FieldValue
                        {
                            FieldName = RFCFields.Manager.InternalName,
                            Value =  new SPFieldUserValue(currentWeb,user.ID,user.LoginName)
                        }
                    }
                }
            };

        }


        private static List<ListItemDefinition> GetTypeDefinitions(List<ListItemDefinition> listItemDefinitions)
        {
            return new List<ListItemDefinition>
            {
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.RfcType.Name,
                    Title = "Обычное", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
                    Values = new   List<FieldValue>
                    {
                        new FieldValue
                        {
                            FieldName = RFCFields.Category.InternalName,
                            Value =  new SPFieldLookupValue(1,"Телекоммуникационные услуги")
                        }
                    }
                },
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.RfcType.Name,
                    Title = "Аварийное", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
                    Values = new   List<FieldValue>
                    {
                        new FieldValue
                        {
                            FieldName = RFCFields.Category.InternalName,
                            Value =  new SPFieldLookupValue(1,"Телекоммуникационные услуги")
                        }
                    }
                },
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.RfcType.Name,
                    Title = "Обычное", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
                    Values = new   List<FieldValue>
                    {
                        new FieldValue
                        {
                            FieldName = RFCFields.Category.InternalName,
                            Value = new SPFieldLookupValue(2,"Внутренней автоматизации")
                        }
                    }
                },
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.RfcType.Name,
                    Title = "Глобальное", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
                    Values = new   List<FieldValue>
                    {
                        new FieldValue
                        {
                            FieldName = RFCFields.Category.InternalName,
                            Value =  new SPFieldLookupValue(2,"Внутренней автоматизации")
                        }
                    }
                },new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.RfcType.Name,
                    Title = "Аварийное", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
                    Values = new   List<FieldValue>
                    {
                        new FieldValue
                        {
                            FieldName = RFCFields.Category.InternalName,
                            Value =  new SPFieldLookupValue(2,"Внутренней автоматизации")
                        }
                    }
                }

            };
        }

        private static List<ListItemDefinition> GetListItemDefinitions()
        {
            return new List<ListItemDefinition>
            {
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.RfcCategory.Name,
                    Title = "Телекоммуникационные услуги", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
                },
                new ListItemDefinition
                {
                    ContentTypeName = RFCContentType.RfcCategory.Name,
                    Title = "Внутренняя автоматизация", Overwrite = false, UpdateOverwriteVersion =  false, SystemUpdateIncrementVersionNumber = false, SystemUpdate = true,
                }
            };
        }
    }
}
