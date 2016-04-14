using Microsoft.SharePoint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSys.Common.ReplaceUsers
{
    public class Settings
    {
        public string Url { get; set; }
        public SPWeb web { get; set; }

        public string OldUser { get; set; }
        public string NewUser { get; set; }

        public string ListNames { get; set; }
        public string FieldNames { get; set; }
        public string RoleListNames { get; set; }
        public string RoleFieldNames { get; set; }
        public string BossListNames { get; set; }
        public string BossFieldNames { get; set; }

        public bool ChangeUsers { get; set; }
        public bool UpdateTasks { get; set; }
        public bool UpdateRoles { get; set; }
        public bool UpdateBosses { get; set; }

        public Settings(SPWeb spWeb)
        {
            this.web = spWeb;
            this.Url = "http://localhost/";
            this.OldUser = "system\\sharepoint";
            this.NewUser = "system\\sharepoint";

            this.ChangeUsers = false;
            this.UpdateBosses = false;
            this.UpdateRoles = true;
            this.UpdateTasks = true;

            this.ListNames = "Рабочие документы;Договора";
            this.FieldNames = "Текущий исполнитель;Инициатор;Координатор";

            this.RoleListNames = "Пользователи и роли";
            this.RoleFieldNames = "Учетная запись.AD";

            this.BossListNames = "Руководители и лимиты";
            this.BossFieldNames = "Руководитель подразделения";
        }
    }
}
