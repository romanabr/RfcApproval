<%
Why do I get "Could not load the assembly '$SharePoint.Project.AssemblyFullName$'. Make sure that it is compiled before accessing the page" for ASHX handler?

http://social.technet.microsoft.com/Forums/en/sharepoint2010programming/thread/a69a09de-c928-4570-bef3-ae446cb6eac6
 %>


<%@ Assembly Name="$SharePoint.Project.AssemblyFullName$" %>
<%@ Assembly Name="Microsoft.SharePoint, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c" %>
<%@ WebHandler Language="C#"  Class="PSE.CustomUpload.MultipleUploadHandler" %>