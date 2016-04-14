using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.SharePoint;

namespace RedSys.Common.Workflow
{   
    public class BranchInfo
    {
        public BranchInfo()
        {
            Approved = true;
            Delegted = false;
            this.User = new UserInfo();
            AdditionalUsers = new List<UserInfo>();
            this.CompleteDate = DateTime.MinValue;
            TaskId = 0;
        }

        public string Name { get; set; }
        public string TaskText { get; set; }
        public string RoleName { get; set; }
        public string TaskType { get; set; }
        public string Comment { get; set; }
        public string MoidifiedBy { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CompleteDate { get; set; }
        public bool Approved { get; set; }
        public bool IsForcedAgreed { get; set; }
        public bool Delegted { get; set; }

        public int TaskId { get; set; }
        public int Step { get; set; }
        public Step UserStep { get; set; }

        public UserInfo User { get; set; }
        public List<UserInfo> AdditionalUsers { get; set; }

        public XmlNode Save(XmlDocument doc, XmlElement node)
        {
            XmlAttribute prop = doc.CreateAttribute("RoleName");
            prop.Value = RoleName;
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("Name");
            prop.Value = Name;
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("TaskText");
            prop.Value = TaskText;
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("TaskType");
            prop.Value = TaskType;
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("Comment");
            prop.Value = Comment;
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("MoidifiedBy");
            prop.Value = MoidifiedBy;
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("DueDate");
            prop.Value = DueDate.ToString();
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("StartDate");
            prop.Value = StartDate.ToString();
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("CompleteDate");
            prop.Value = CompleteDate.ToString();
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("MoidifiedBy");
            prop.Value = MoidifiedBy;
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("Approved");
            prop.Value = Approved.ToString();
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("IsForcedAgreed");
            prop.Value = IsForcedAgreed.ToString();
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("Delegted");
            prop.Value = Delegted.ToString();
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("Step");
            prop.Value = UserStep == null ? "0" : UserStep.ID.ToString();
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("TaskId");
            prop.Value = TaskId.ToString();
            node.Attributes.Append(prop);

            node.AppendChild(User.Save(doc, doc.CreateElement("User")));

            XmlElement addu = doc.CreateElement("AdditionalUsers"); 
            foreach (UserInfo adduser in AdditionalUsers)
            {
                addu.AppendChild(adduser.Save(doc, doc.CreateElement("User")));
            }
            node.AppendChild(addu);
            return node;
        }

        public void Load(XmlDocument doc, XmlElement node, SPWeb web)
        {
            this.RoleName = node.Attributes["RoleName"].Value;
            this.TaskText = node.Attributes["TaskText"].Value;
            this.Name = node.Attributes["Name"].Value;
            this.TaskType = node.Attributes["TaskType"].Value;
            this.Comment = node.Attributes["Comment"].Value;
            this.MoidifiedBy = node.Attributes["MoidifiedBy"].Value;
            this.DueDate = DateTime.Parse(node.Attributes["DueDate"].Value);
            this.StartDate = DateTime.Parse(node.Attributes["StartDate"].Value);
            this.CompleteDate = DateTime.Parse(node.Attributes["CompleteDate"].Value);
            this.Approved = bool.Parse(node.Attributes["Approved"].Value);
            if (node.Attributes["Delegted"] != null)
                this.Delegted = bool.Parse(node.Attributes["Delegted"].Value);
            this.IsForcedAgreed = bool.Parse(node.Attributes["IsForcedAgreed"].Value);
            this.Step = int.Parse(node.Attributes["Step"].Value);
            if (node.Attributes["TaskId"] != null)
                this.TaskId = int.Parse(node.Attributes["TaskId"].Value);
            this.UserStep = new Step(this.Step, web);
            User.Load(node["User"]);
            if (node["AdditionalUsers"] != null)
            {
                foreach (XmlElement cn in node["AdditionalUsers"].ChildNodes)
                {
                    UserInfo ui = new UserInfo();
                    ui.Load(cn);
                    AdditionalUsers.Add(ui);
                }
            }
        }
    }

    public class UserInfo
    {
        public string UserName { get; set; }
        public string RealUserName { get; set; }

        public XmlNode Save(XmlDocument doc, XmlNode node)
        {
            XmlAttribute prop = doc.CreateAttribute("UserName");
            prop.Value = UserName;
            node.Attributes.Append(prop);

            prop = doc.CreateAttribute("RealUserName");
            prop.Value = RealUserName;
            node.Attributes.Append(prop);
            return node;
        }

        public void Load(XmlNode node)
        {
            this.UserName = node.Attributes["UserName"].Value;
            this.RealUserName = node.Attributes["RealUserName"].Value;
        }
    }
}
