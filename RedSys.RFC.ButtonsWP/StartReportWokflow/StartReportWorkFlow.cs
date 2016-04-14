using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml;
using RedSys.RFC.Core.Helper;
using CamlexNET;
using Microsoft.Office.DocumentManagement.DocumentSets;
using Microsoft.Office.Server.Audience;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.WebControls;
using WP_Button.Code.Translate;
using RedSys.Common.Workflow;
using RedSys.RFC.Data.Fields;
using RedSys.RFC.Data;
using RedSys.RFC.Data.Const;

namespace ReportButton.StartReportWorkflow
{
	[ToolboxItem(false)]
	public class StartReportWorkflow : WebPart
	{
		private SPLinkButton _copyButton;
		private SPLinkButton _exportButton;
		private readonly Label _lblInfo;
		private string _oldstatus;
		private SPLinkButton _startButton;
		private SPLinkButton _stopButton;
		private SPLinkButton _toArchieveButton;
		private SPLinkButton _onpreworkButton;
		private Workflow _wf;

		public StartReportWorkflow()
		{
			_lblInfo = new Label
			{
				ForeColor = Color.Green,
				Visible = false
			};
			TemplatesListName = "Шаблоны документов";
		}

		private string EnsureParentFolder(SPWeb parentWeb, string destinUrl)
		{
			destinUrl = parentWeb.GetFile(destinUrl).Url;

			var index = destinUrl.LastIndexOf("/");
			var parentFolderUrl = string.Empty;

			if (index <= -1) return parentFolderUrl;
			parentFolderUrl = destinUrl.Substring(0, index);

			var parentFolder
				= parentWeb.GetFolder(parentFolderUrl);

			if (!parentFolder.Exists)
			{
				SPSecurity.RunWithElevatedPrivileges(delegate
				{
					using (var site = new SPSite(parentWeb.Url))
					{
						using (var web = site.OpenWeb())
						{
							var curFolder = web.RootFolder;
							foreach (var folder in parentFolderUrl.Split('/'))
							{
								if (!(parentWeb.Lists[parentFolder.ParentListId] is SPDocumentLibrary)) continue;
								web.AllowUnsafeUpdates = true;
								curFolder = curFolder.SubFolders.Add(folder);
								web.AllowUnsafeUpdates = false;
							}
						}
					}
				});
			}
			return parentFolderUrl;
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			EnsureChildControls();
		}

		protected override void CreateChildControls()
		{
			if (ShowStartWorkflow || ShowWFStop)
				_wf = new Workflow(SPContext.Current.Item as SPListItem);
			try
			{
				var curItem = SPContext.Current.ListItem;
				string status = curItem.GetFieldValue(WorkflowFields.WorkflowStage.FieldInternalName).ToLower().Trim();

				SPUser manager = curItem.GetFieldValueUser(RFCFields.Manager.InternalName);
				SPUser author = curItem.GetFieldValueUser(SPBuiltInFieldId.Author);
				SPUser currentUser = SPContext.Current.Web.CurrentUser;
				SPWeb currentWeb = SPContext.Current.Web;
				if (null == curItem)
				{
					Controls.Add(new Label
					{
						Text = "SPContext.Current.ListItem is null",
						ForeColor = Color.Red
					});
					return;
				}
				ShowHomeButton();
				ShowStartButton(status);
				ShowCopyButton();

				if (string.IsNullOrEmpty(StatusMoveToArchieve))
				{
					StatusMoveToArchieve = "New";
				}
				if (ShowMoveToArchieve &&
					!string.IsNullOrEmpty(StatusMoveToArchieve) &&
					!string.IsNullOrEmpty(status)
					&&
					StatusMoveToArchieve.ToLower().Trim().Contains(status))
				{
					_toArchieveButton = new SPLinkButton
					{
						ImageUrl = "~/_layouts/15/images/WP Button/icToArchieve1.png",
						OnClientClick = "return confirm('Вы уверены что хотите отправить запрос в архив?');"
					};
					_toArchieveButton.Click += toArchieveButton_Click;
					Controls.Add(_toArchieveButton);
					Controls.Add(new LiteralControl("&nbsp"));
				}
				if (ShowWFStop && _wf.InProgress)
				{
					var fields = !string.IsNullOrEmpty(StopWFUserFields) ? StopWFUserFields.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries) : new string[] { };

					var hasAccess = false;
					if(currentUser != null)
					foreach (var name in fields)
					{
						if (string.IsNullOrEmpty(curItem.GetFieldValue(name))) continue;
						var val  = curItem.GetFieldValueUser(name);
						if (val == null || val.ID != currentUser.ID) continue;
						hasAccess = true;
						break;
					}

					//TODO Сделать метод получиения группы администраторов
					if (hasAccess || currentWeb.IsCurrentUserMemberOfGroup(currentWeb.AssociatedOwnerGroup.ID) || currentUser.ID == currentWeb.Site.SystemAccount.ID)
					{
						_stopButton = new SPLinkButton
						{
							ImageUrl = "~/_layouts/15/images/ReportWP/icbreakEng.png",
							OnClientClick = "return confirm('Вы уверены что хотите отменить процесс?');return false;"
						};

						_stopButton.Click += stopButton_Click;
						Controls.Add(_stopButton);
						Controls.Add(new LiteralControl("&nbsp"));
						Controls.Add(new LiteralControl("&nbsp"));
					}
				}
				if (ShowOnRework)
				{

					bool workflowStatusOnRework = false;
					if (!string.IsNullOrEmpty(WorkflowStatusOnRework))
					{
						workflowStatusOnRework = WorkflowStatusOnRework.ToLower().Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Contains(status);
					}
					if(workflowStatusOnRework)
					{
						if(currentUser.ID == manager.ID || currentUser.ID == author.ID)
						{
							_onpreworkButton = new SPLinkButton
							{
								ImageUrl = "~/_layouts/15/images/ReportWP/icrecallEng.png",
								OnClientClick = "return confirm('Вы уверены что хотите отправить запрос на доработку?');return false;",
								ToolTip ="On rework"
							};
							_onpreworkButton.Click += onpreworkButton_Click;
							Controls.Add(_onpreworkButton);
							Controls.Add(new LiteralControl("&nbsp"));
							Controls.Add(new LiteralControl("&nbsp"));
						}
					}
				}
				if (HideWFButton)
				{
					var ribbon = SPRibbon.GetCurrent(Page);
					ribbon.TrimById("Ribbon.ManageDocumentSet.MDS.Manage.Workflows");
				}
				if (EditBtn)
				{
					var editLink = new SPLinkButton
					{
						ImageUrl = "/_layouts/15/images/ReportWP/changeEng.png",
						Width = 60,
						Height = 60
					};
					var editLinkText = SPContext.Current.List.DefaultEditFormUrl + "?ID=" + SPContext.Current.ListItem.ID +
									   "&ContentTypeId=" + SPContext.Current.ListItem.ContentTypeId + "&Source=" +
									   HttpUtility.UrlEncode(Context.Request.Url.ToString());
					editLink.OnClientClick = "javascript:OpenDialog('" + editLinkText + "');return false;";
					Controls.Add(editLink);
				}


				var statusesroute = ApprovalRouteState != null ? ApprovalRouteState.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries) : null;


				if (ShowApprovalRoute && statusesroute != null && statusesroute.Contains(status))
				{
					var qs = new QueryString(ApprovalRouteURL);
					qs["DocID"] = SPContext.Current.ListItem.ID.ToString();
					var approvalListButton = new SPLinkButton
					{
						ImageUrl = "~/_layouts/15/images/WP Button/icRoute.png",
						OnClientClick = "javascript:OpenDialog('" + QueryString.ForceUrlToBeReloaded(qs.ToString()) +
										"');return false;"
					};
					Controls.Add(approvalListButton);
					Controls.Add(new LiteralControl("&nbsp"));
				}

				if (ShowApprovalListInExternalApp)
				{
					var statuses = ShowApprovalListInWordStatuses.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
					var curStatus = SPContext.Current.ListItem.GetFieldValue(WorkflowFields.WorkflowStage.FieldInternalName);
					if (statuses.Contains(curStatus))
					{
						var scr = new Literal();
						scr.Text = "<script type='text/javascript' src='/_layouts/15/Ensol.WPButtonWP/SP15ModalDialog.js'></script>";
						Controls.Add(scr);

						var callMSWordButton = new SPLinkButton
						{
							ToolTip = Translate.OUTPUTINTTOMSWORD,
							ImageUrl = "/_layouts/15/images/WP Button/PrintForm.png",
							Width = 60,
							Height = 60,
							OnClientClick = "return confirm('Create printed form by template?');return false;"
						};
						//callMSWordButton.Click += new EventHandler(callMSWordButton_Click);
						callMSWordButton.OnClientClick =
							"openInDialog(300,270,false,true,false,'/_layouts/15/Ensol.WPButtonWP/DocTemplatePage.aspx?IsDlg=1&cid=" +
							curItem.ID + "');return false;";
						Controls.Add(callMSWordButton);
						Controls.Add(new LiteralControl("&nbsp"));
					}
				}
			}
			catch (Exception ex)
			{
				ExceptionHelper.DUmpException(ex);
				var lbl = new Label();
				lbl.Text = "Ошибка: ";
				lbl.Text += ex.Message;
				Controls.Add(lbl);
			}

			if (ShowLinkButton)
			{
				var curStatus = SPContext.Current.ListItem.GetFieldValue(WorkflowFields.WorkflowStage.FieldInternalName);
				if (string.IsNullOrEmpty(LinkButtonStatuses) || LinkButtonStatuses.Contains(curStatus))
				{
					var qs = new QueryString(LinkButtonURL);
					qs["DocID"] = SPContext.Current.ListItem.ID.ToString();
					var linkb = new SPLinkButton();
					//linkb.NavigateUrl = QueryString.ForceUrlToBeReloaded(qs.ToString());
					linkb.ImageUrl = "~/_layouts/15/images/WP Button/Actions-insert-link.png";
					linkb.OnClientClick = "javascript:openDialog2(GetBaseOptions('Create link','" +
										  QueryString.ForceUrlToBeReloaded(qs.ToString()) + "&IsDlg=1',700, 700));return false;";
					Controls.Add(linkb);
					Controls.Add(new LiteralControl("&nbsp"));
				}
			}
		}

		private void onpreworkButton_Click(object sender, EventArgs e)
		{
			SPSecurity.RunWithElevatedPrivileges(delegate
			{
				using (var oSite = new SPSite(SPContext.Current.Web.Url))
				{
					using (var oWeb = oSite.OpenWeb())
					{
						try
						{
							oWeb.AllowUnsafeUpdates = true;
							var oLst = oWeb.GetListExt(SPContext.Current.List.RootFolder.Url);
							if (Page.Request.QueryString["ID"] != null)
							{
								var curItem = oLst.GetItemById(int.Parse(Page.Request.QueryString["ID"]));
								_oldstatus = curItem.GetFieldValue(WorkflowFields.WorkflowStage.FieldInternalName);
								_wf = new Workflow(curItem);
								_wf.Stop(SPContext.Current.Web.CurrentUser);

								curItem[WorkflowFields.WorkflowCurrentUser.FieldInternalName] = string.Empty;
								curItem[WorkflowFields.WorkflowStage.FieldInternalName] = RFCStatus.RECALLED;
								curItem.SystemUpdate(false);

								if (ChangeChild)
								{
									LogData(curItem, RFCStatus.RECALLED);
								}
							}
						}
						catch (ThreadAbortException)
						{
						}
						catch (Exception ex)
						{
							ExceptionHelper.DUmpException(ex);
							throw;
						}
					}
				}
			});
			Page.Response.Redirect(Page.Request.Url.AbsoluteUri);

		}

		private void ShowCopyButton()
		{
			if (ShowCopy && CopyField != "" && SPContext.Current.Web.SiteGroups[CopyField] != null &&
								SPContext.Current.Web.SiteGroups[CopyField].ContainsCurrentUser)
			{
				_copyButton = new SPLinkButton
				{
					ImageUrl = "~/_layouts/15/images/WP Button/iccopy.png",
					ToolTip = "Copy docset",
					OnClientClick = "return confirm('Создать дубликат запроса?');return false;"
				};
				
				Controls.Add(_copyButton);
				Controls.Add(new LiteralControl("&nbsp"));
				Controls.Add(new LiteralControl("&nbsp"));
			}
		}

		private void ShowStartButton(string status)
		{
			if (ShowStartWorkflow && !_wf.InProgress
								&&
								(string.IsNullOrEmpty(status) || string.IsNullOrEmpty(StatusStartWF) ||
								 StatusStartWF.ToLower().Trim().Contains(status)))
			{
				_startButton = new SPLinkButton
				{
					ImageUrl = "~/_layouts/15/images/ReportWP/icStartApproveEng.png",
					OnClientClick = "return confirm('Do you want to submit the report? The approval process will start automatically if approvers are specified for this report.');return false;"
				};
				_startButton.Click += startButton_Click;
				Controls.Add(_startButton);
				Controls.Add(new LiteralControl("&nbsp"));
				Controls.Add(new LiteralControl("&nbsp"));
			}
		}

		private void ShowHomeButton()
		{
			if (AddHomeBtn)
			{
				var HomeBtn = new SPLinkButton
				{
					ImageUrl = "~/_layouts/15/images/ReportWP/mainEng.png",
					Width = 60,
					Height = 60,
					NavigateUrl = SPContext.Current.Web.Url
				};
				Controls.Add(new LiteralControl(@"&nbsp<script>function DisableTitleRow(){$('#titlerow').css('display','none');} ExecuteOrDelayUntilScriptLoaded(DisableTitleRow,'SP.js');</script>"));
				//id="titlerow"/
				Controls.Add(HomeBtn);
				Controls.Add(new LiteralControl("&nbsp"));
			}
		}



		private void exportButton_Click(object sender, EventArgs e)
		{
			Controls.Add(new LiteralControl("<br/>"));
			var lbl = new Label();
			var curItem = SPContext.Current.ListItem;
			var emptyFields = CheckExportFields(curItem, RequieredFields1C);
			if (!string.IsNullOrEmpty(emptyFields))
			{
				lbl.Text = string.Format("Fields {0} must be filled.", emptyFields);
				lbl.ForeColor = Color.Red;
			}
			else
				SPSecurity.RunWithElevatedPrivileges(delegate
				{
					using (var site = new SPSite(curItem.Web.Site.ID))
					{
						using (var web = site.OpenWeb(curItem.Web.ID))
						{
							try
							{
								web.AllowUnsafeUpdates = true;
								curItem = web.Lists[curItem.ParentList.ID].GetItemById(curItem.ID);
								//Common.ExportData(curItem, "Входящие документы", "Ensol.DocExportInc.SQLConnectionString","Ensol.DocExportInc.DBTableName", false);
								curItem.Update();
								lbl.Text = Translate.DOCSUCCESFULEXPORTED;
								lbl.ForeColor = Color.Green;
							}
							catch (Exception ex)
							{
								ExceptionHelper.DUmpException(ex);
								lbl.Text = "Ошибка: " + ex.Message;
								lbl.ForeColor = Color.Red;
							}
						}
					}
				});
			Controls.Add(lbl);
		}

		private string CheckExportFields(SPListItem spli, string requieredFields)
		{
			var emtyFields = string.Empty;
			if (!string.IsNullOrEmpty(requieredFields))
				foreach (var s in requieredFields.Split(new[] {';'},StringSplitOptions.RemoveEmptyEntries))
					if (spli.ParentList.Fields.ContainsField(s) && (spli[s] == null || spli[s].ToString() == string.Empty))
						emtyFields =string.IsNullOrEmpty( emtyFields )? s : emtyFields + ";" + s;
			return emtyFields;
		}

		private void toArchieveButton_Click(object sender, EventArgs e)
		{
			var haserror = false;
			SPSecurity.RunWithElevatedPrivileges(delegate
			{
				var curItem = SPContext.Current.ListItem;
				var startFlag = false;
				using (var site = new SPSite(curItem.Web.Site.ID))
				{
					using (var web = site.OpenWeb(curItem.Web.ID))
					{
						try
						{
							// for security purposes get SPListItem again
							curItem = web.Lists[curItem.ParentList.ID].GetItemById(curItem.ID);

							var routs = Helper.GetItemsByValue(web, "Маршруты", "Тип контента=" + curItem.ContentType.Name);


							if (!string.IsNullOrEmpty(ToArchieveStatusField) && string.IsNullOrEmpty(ToArchieveStatus))
							{
								var statuses = ToArchieveStatus.Split(';');

								foreach (var status in statuses)
								{
									if (curItem[ToArchieveStatusField].ToString() == status)
									{
										startFlag = true;
									}
								}
							}
							else
								startFlag = true;

							if (!MoveParent && !MoveChild)
							{
								Controls.Add(new LiteralControl("<br/>"));
								var lbl = new Label();
								lbl.Text = Translate.MOVINGISDISABLE;
								lbl.ForeColor = Color.Red;
								Controls.Add(lbl);

								return;
							}

							if (routs == null || routs.Count == 0)
							{
								Controls.Add(new LiteralControl("<br/>"));
								var lbl = new Label();
								lbl.Text = Translate.UNSPECIFYPATH;
								lbl.ForeColor = Color.Red;
								Controls.Add(lbl);

								return;
							}

							if (!startFlag)
							{
								Controls.Add(new LiteralControl("<br/>"));
								var lbl = new Label();
								lbl.Text = "Document Set cann't be moved: " + ToArchieveStatusField + "=" + curItem[ToArchieveStatusField];
								lbl.ForeColor = Color.Red;
								Controls.Add(lbl);

								return;
							}

							using (var eventReceiverManager = new EventReceiverManager(true))
							{
								if (MoveParent)
								{
									curItem[WorkflowFields.WorkflowStage.FieldInternalName] = "Архивный";
									curItem.Web.AllowUnsafeUpdates = true;
									curItem.Update();
									Helper.ToArchive(curItem, false, false, null);
								}

								if (MoveChild)
								{
									if (!string.IsNullOrEmpty(ChildLib))
									{
										var viewLists = string.Empty;
										var viewFields = string.Empty;

										SPList childList = null;
										foreach (var str in ChildLib.Split(';'))
										{
											childList = curItem.Web.Lists[str];
											viewLists += "<List ID='" + childList.ID + "'/>";
										}

										if (childList != null)
										{
											var camlexSt = "<Where><And>><Eq><FieldRef Name='IsDocumentSet'/><Value Type='Boolean'>1</Value></Eq>" +
											               "<Contains><FieldRef Name='" + childList.Fields[KeyField].InternalName +
											               "'/><Value Type='Text'>" + curItem[ChildKeyField] + "</Value></Contains>" +
											               "</And></Where><OrderBy><FieldRef Name='ID' /></OrderBy>";
											viewLists = "<Lists>" + viewLists + "</Lists>";
											viewFields += "<FieldRef Name='Title' Nullable='TRUE'/>";

											var sdq = new SPSiteDataQuery
											{
												Lists = viewLists,
												Query = camlexSt,
												ViewFields = viewFields,
												Webs = "<Webs Scope='SiteCollection'/>"
											};

											var dt = curItem.Web.GetSiteData(sdq);
											foreach (DataRow dr in dt.Rows)
											{
												try
												{
													var addlist = curItem.Web.Lists[new Guid(dr["ListId"].ToString())];
													var additem = addlist.GetItemById(int.Parse(dr["ID"].ToString()));
													Helper.ToArchive(additem, false, false, null);
												}
												catch (Exception ex)
												{
													ExceptionHelper.DUmpException(ex);
													throw ex;
												}
											}
										}
									}
								}

								eventReceiverManager.StartEventReceiver();
							}
						}
						catch (ThreadAbortException ex)
						{
							haserror = true;
							var lbl = new Label {Text = "Ошибка: " + ex.Message};
							Controls.Add(lbl);
						}
						catch (Exception ex)
						{
							haserror = true;
							ExceptionHelper.DUmpException(ex);
							var lbl = new Label {Text = "Ошибка: " + ex.Message};
							Controls.Add(lbl);
						}
					}
				}
			});

			if (!haserror)
			{
				var url = "/" + SPContext.Current.ListItem.ParentList.RootFolder.Name;
				SPUtility.Redirect(url, SPRedirectFlags.Default, HttpContext.Current);
			}
		}

		private void CheckUnique()
		{
			var result = "";
			var curItem = SPContext.Current.ListItem;
			var oList = SPContext.Current.List;

			var IDs = new List<int> {SPContext.Current.Item.ID};
			var checkQuery = new SPQuery
			{
				Query = "<Where><Eq><FieldRef Name=\"Title\" /><Value Type=\"Text\">" + curItem.Title +
				        "</Value></Eq></Where>",
				ViewAttributes = "Scope=\"RecursiveAll\""
			};
			var otherItems = oList.GetItems(checkQuery);
			var localresult = string.Empty;
			foreach (SPListItem oitem in otherItems)
			{
				if (!IDs.Contains(oitem.ID))
				{
					IDs.Add(oitem.ID);
					localresult += "<a href='" + oList.DefaultDisplayFormUrl + "?ID=" + oitem.ID + "'>" + oitem.Title + "</a><br/>";
				}
			}
			if (localresult != "")
			{
				result += "Title:<br/>" + localresult;
			}

			checkQuery = new SPQuery
			{
				ViewAttributes = "Scope=\"RecursiveAll\"",
				Query = "<Where><Eq><FieldRef Name=\"INN\" /><Value Type=\"Text\">" + curItem["ИНН"] +
				        "</Value></Eq></Where>"
			};
			otherItems = oList.GetItems(checkQuery);
			localresult = string.Empty;
			foreach (SPListItem oitem in otherItems)
			{
				if (IDs.Contains(oitem.ID)) continue;
				IDs.Add(oitem.ID);
				localresult += "<a href='" + oList.DefaultDisplayFormUrl + "?ID=" + oitem.ID + "'>" + oitem.Title + "</a><br/>";
			}
			if (!string.IsNullOrEmpty((localresult)))
			{
				result += "ИНН:<br/>" + localresult;
			}

			checkQuery = new SPQuery
			{
				ViewAttributes = "Scope=\"RecursiveAll\"",
				Query = "<Where><Eq><FieldRef Name=\"KPP\" /><Value Type=\"Text\">" + curItem["КПП"] +
				        "</Value></Eq></Where>"
			};
			otherItems = oList.GetItems(checkQuery);
			localresult = string.Empty;
			foreach (SPListItem oitem in otherItems)
			{
				if (IDs.Contains(oitem.ID)) continue;
				IDs.Add(oitem.ID);
				localresult += "<a href='" + oList.DefaultDisplayFormUrl + "?ID=" + oitem.ID + "'>" + oitem.Title + "</a><br/>";
			}
			if (localresult != "")
			{
				result += "КПП:<br/>" + localresult;
			}

			if (result.Trim() == "")
				result = Translate.ITEMISUNIQUE;
			else
			{
				result = Translate.ITEMSWITHSIMILARPARAMETR + ":<br/>" + result;
			}
			result = "<br/>" + result;
			Controls.Add(new LiteralControl(result));
			Controls.Add(new
				LiteralControl(@"&nbsp<script>
function EnableSave()
{$('#ctl00_m_g_a310a596_0d47_4922_8362_b11650f8ffd1_ctl00_toolBarTbl_RightRptControls_ctl00_ctl00_diidIOSaveItem').removeAttr('disabled');}
ExecuteOrDelayUntilScriptLoaded(EnableSave,'SP.js');
</script>"));
		}

		private void stopButton_Click(object sender, EventArgs e)
		{
			SPSecurity.RunWithElevatedPrivileges(delegate
			{
				using (var oSite = new SPSite(SPContext.Current.Web.Url))
				{
					using (var oWeb = oSite.OpenWeb())
					{
						try
						{
							oWeb.AllowUnsafeUpdates = true;
							var oLst = oWeb.GetListExt(SPContext.Current.List.RootFolder.Url);
							if (Page.Request.QueryString["ID"] != null)
							{
								var curItem = oLst.GetItemById(int.Parse(Page.Request.QueryString["ID"]));
								_oldstatus = curItem.GetFieldValue(WorkflowFields.WorkflowStage.FieldInternalName);
								_wf = new Workflow(curItem);
								_wf.Stop(SPContext.Current.Web.CurrentUser);

								List<SPUser> currentApprover = curItem.GetFieldValueUserCollection(WorkflowFields.WorkflowCurrentUser.FieldInternalName);
								
								curItem[WorkflowFields.WorkflowCurrentUser.FieldInternalName] = string.Empty;
								curItem[WorkflowFields.WorkflowStage.FieldInternalName] = RFCStatus.RECALLED;
								curItem.SystemUpdate(false);

								if (ChangeChild)
								{
									LogData(curItem,RFCStatus.CANCELLED);
								}
							}
						}
						catch (ThreadAbortException)
						{
						}
						catch (Exception ex)
						{
							ExceptionHelper.DUmpException(ex);
							throw;
						}
					}
				}
			});
			Page.Response.Redirect(Page.Request.Url.AbsoluteUri);
		}

		private void startButton_Click(object sender, EventArgs e)
		{
			var curItem = SPContext.Current.ListItem;

			#region Проверки перед запуском...

			var emptyFields = CheckExportFields(curItem, RequieredFieldsWF);
			if (!string.IsNullOrEmpty(emptyFields))
			{
				Controls.Add(new LiteralControl("<br/>"));
				var lbl = new Label
				{
					Text = "Fields " + emptyFields + " must be filled.",
					ForeColor = Color.Red
				};
				Controls.Add(lbl);
				return;
			}
			if (CountFiles)
			{
				if (curItem.Folder.ItemCount == 0)
				{
					ExceptionHelper.DUmpExceptionWithJsDependentAndNoRedirect(null,
						"The report can’t be submitted. Please upload the reporting files or links and click “Submit” button again.", this, "jquery.easytabs.min.js");
					//this.Controls.Add(new LiteralControl("<br/>"));
					//Label lbl = new Label();
					//lbl.Text = "Процесс согласования не может быть запущен! В согласуемом наборе документов нет ни одного документа!";
					//lbl.ForeColor = System.Drawing.Color.Red;
					//this.Controls.Add(lbl);
					return;
				}
			}

			#endregion

			SPUtility.ValidateFormDigest();

			SPSecurity.RunWithElevatedPrivileges(delegate
			{
				using (var site = new SPSite(curItem.Web.Url))
				{
					using (var web = site.OpenWeb())
					{
						var wfItem = web.Lists[curItem.ParentList.ID].GetItemById(curItem.ID);
						try
						{
							_oldstatus = curItem.GetFieldValue(WorkflowFields.WorkflowStage.FieldInternalName);
							using (var eventReceiverManager = new EventReceiverManager(true))
							{
								if (MoveDocSetBeforeStart)
								{
									curItem = Helper.ToArchive(wfItem, false, false, null);
									Thread.Sleep(2000);
									wfItem = web.Lists[curItem.ParentList.ID].GetItemById(curItem.ID);
								}
								eventReceiverManager.StartEventReceiver();
							}

							_wf = new Workflow(wfItem);
							_wf.StartNew(SPContext.Current.Web.CurrentUser);

							if (ChangeChild)
							{
								LogData(wfItem, "Запуск процесса");
							}

							Page.Response.Redirect(Page.Request.Url.AbsoluteUri);
							SPUtility.Redirect(WorkFlowRedirectUrl, SPRedirectFlags.Default, HttpContext.Current);
						}
						catch (ThreadAbortException)
						{
						}
						catch (Exception ex)
						{
							ExceptionHelper.DUmpException(ex, "Ошибка: " + ex.Message, this);
						}
					}
				}
			});
		}

		public void LogData(SPListItem oItem, string Text)
		{
			var mainItem = oItem;
			if (!string.IsNullOrEmpty(ChildLib))
			{
				var viewLists = string.Empty;
				var viewFields = string.Empty;

				SPList childList = null;
				foreach (var str in ChildLib.Split(';'))
				{
					childList = oItem.Web.Lists[str];
					viewLists += string.Format("<List ID='{0}'/>", childList.ID);
				}

				if (childList != null)
				{
					var camlexSt = "<Where><And><And><Eq><FieldRef Name='" + WorkflowFields.WorkflowStage.FieldInternalName +
								   "'/><Value Type='Text'>" + _oldstatus +
								   "</Value></Eq><Eq><FieldRef Name='IsDocumentSet'/><Value Type='Boolean'>1</Value></Eq></And>" +
								   "<Contains><FieldRef Name='" + childList.Fields[KeyField].InternalName + "'/><Value Type='Text'>" +
								   mainItem[ChildKeyField] + "</Value></Contains>" +
								   "</And></Where><OrderBy><FieldRef Name='ID' /></OrderBy>";
					viewLists = string.Format("<Lists>{0}</Lists>", viewLists);
					viewFields += "<FieldRef Name='Title' Nullable='TRUE'/>";

					var sdq = new SPSiteDataQuery
					{
						Lists = viewLists,
						Query = camlexSt,
						ViewFields = viewFields,
						Webs = "<Webs Scope='SiteCollection'/>"
					};

					var dt = oItem.Web.GetSiteData(sdq);
					foreach (DataRow dr in dt.Rows)
					{
						try
						{
							var addlist = oItem.Web.Lists[new Guid(dr["ListId"].ToString())];
							var additem = addlist.GetItemById(int.Parse(dr["ID"].ToString()));
							var w = new Workflow(additem);
							if (Text == RFCStatus.RECALLED)
							{
								additem[WorkflowFields.WorkflowCurrentUser.FieldInternalName] = string.Empty;
								additem[WorkflowFields.WorkflowStage.FieldInternalName] = RFCStatus.RECALLED;
								additem.SystemUpdate(false);
								w.Stop(SPContext.Current.Web.CurrentUser);
							}
							else if(Text == RFCStatus.CANCELLED)
							{
								additem[WorkflowFields.WorkflowCurrentUser.FieldInternalName] = string.Empty;
								additem[WorkflowFields.WorkflowStage.FieldInternalName] = RFCStatus.CANCELLED;
								additem.SystemUpdate(false);
								w.Stop(SPContext.Current.Web.CurrentUser);
							}
							else
							{
								w.StartNew(SPContext.Current.Web.CurrentUser);
								Helper.ToArchive(additem, false, false, null);
							}
						}
						catch (Exception ex)
						{
							ExceptionHelper.DUmpException(ex);
							throw ex;
						}
					}
				}
			}
		}

		

		#region ALL WEB PART'S PROPERTIES

		#region Рабочий процесс 

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(false),
		 Category("Рабочий процесс"),
		 WebDisplayName("Выполнять проверку на наличие документов в сете"),
		 WebDescription("Выполнять проверку на наличие документов в сете")]
		public bool CountFiles { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Рабочий процесс"),
		 WebDisplayName("Статусы рабочего процесса, в которых доступна кнопка"),
		 WebDescription("Статусы рабочего процесса, в которых доступна кнопка")]
		public string StatusStartWF { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(false),
		 Category("Рабочий процесс"),
		 WebDisplayName("Запуск рабочего процесса"),
		 WebDescription("Запуск рабочего процесса")]
		public bool ShowStartWorkflow { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(false),
		 Category("Рабочий процесс"),
		 WebDisplayName("Перенос перед запуском рабочего процесса"),
		 WebDescription("Перенос перед запуском рабочего процесса")]
		public bool MoveDocSetBeforeStart { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Рабочий процесс"),
		 WebDisplayName("Url после запуска рабочего процесса"),
		 WebDescription("Url после запуска рабочего процесса")]
		public string WorkFlowRedirectUrl { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(false),
		 Category("Рабочий процесс"),
		 WebDisplayName("Убрать кнопку на риббоне"),
		 WebDescription("Убрать кнопку на риббоне")]
		public bool HideWFButton { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Рабочий процесс"),
		 WebDisplayName("Проверка на заполненность полей"),
		 WebDescription("Проверка на заполненность полей")]
		public string RequieredFieldsWF { get; set; }

		#endregion

		#region Пакет

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Пакет"),
		 WebDisplayName("Обработка пакета"),
		 WebDescription("Обработка пакета")]
		public bool ChangeChild { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Пакет"),
		 WebDisplayName("Библиотеки пакета"),
		 WebDescription("Библиотеки пакета")]
		public string ChildLib { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue("Штрих-код"),
		 Category("Пакет"),
		 WebDisplayName("Поле текущего элемента"),
		 WebDescription("Название поля текущего элемента")]
		public string ChildKeyField { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue("Родительский документ"),
		 Category("Пакет"),
		 WebDisplayName("Поле фильтруемого списка"),
		 WebDescription("Поле фильтруемого списка")]
		public string KeyField { get; set; }

		#endregion

		#region Остановка БП

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Остановка БП"),
		 WebDisplayName("Перечень полей с пользователями, имеющими доступ (Владелец документа;Оператор ОСА)"),
		 WebDescription("Перечень полей с пользователями, имеющими доступ (Владелец документа;Оператор ОСА)")]
		public string StopWFUserFields { get; set; }


		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Остановка БП"),
		 WebDisplayName("Показывать кнопку остановки"),
		 WebDescription("Показывать кнопку остановки")]
		public bool ShowWFStop { get; set; }

		#endregion

		#region Настройки

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Настройки"),
		 WebDisplayName("Отображать кнопку перехода на домашнюю"),
		 WebDescription("Отображать кнопку перехода на домашнюю")]
		public bool AddHomeBtn { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Настройки"),
		 WebDisplayName("Отображать кнопку изменения"),
		 WebDescription("Отображать кнопку изменения")]
		public bool EditBtn { get; set; }

		#endregion

		#region Копирование

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Копирование"),
		 WebDisplayName("Группа, имеющая право на использование"),
		 WebDescription("Группа, имеющая право на использование")]
		public string CopyField { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(false),
		 Category("Копирование"),
		 WebDisplayName("Отображать кнопку"),
		 WebDescription("Отображать кнопку")]
		public bool ShowCopy { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 Category("Копирование"),
		 WebDisplayName("Не копировать поля"),
		 WebDescription("Список полей не подлежащих копированию"),
		 DefaultValue(
			 "Статус рабочего процесса;WFXMLHistory;Замена сканобраза;Лист согласования;Не было согласовано;Связанные документы;Статус рабочего процесса;Статус поручения;Текущий исполнитель;Штрихкод;Штрих-код;"
			 )]
		public string ExcludeFields { get; set; }

		#endregion

		#region Перенос в архив

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Перенос в архив"),
		 WebDisplayName("Имя поля"),
		 WebDescription("Имя поля для проверки")]
		public string ToArchieveStatusField { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Перенос в архив"),
		 WebDisplayName("Перечень значений для фильтра (на согласовании;null;перенос в архив...)"),
		 WebDescription("Перечень значений для фильтра (на согласовании;null;перенос в архив...)")]
		public string ToArchieveStatus { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(false),
		 Category("Перенос в архив"),
		 WebDisplayName("Разрешить перенос в архив"),
		 WebDescription("Разрешить перенос в архив")]
		public bool ShowMoveToArchieve { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue("Новый"),
		 Category("Перенос в архив"),
		 WebDisplayName("Статусы рабочего процесса, в которых доступна кнопка"),
		 WebDescription("Статусы рабочего процесса, в которых доступна кнопка")]
		public string StatusMoveToArchieve { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(false),
		 Category("Перенос в архив"),
		 WebDisplayName("Переносить текущий"),
		 WebDescription("Переносить текущий")]
		public bool MoveParent { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(false),
		 Category("Перенос в архив"),
		 WebDisplayName("Переносить дочерние"),
		 WebDescription("Переносить дочерние")]
		public bool MoveChild { get; set; }

		#endregion

		#region Экспорт в Xml

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(false),
		 Category("Экспорт в Xml"),
		 WebDisplayName("Разрешить экспорт в Xml"),
		 WebDescription("Разрешить экспорт в Xml")]
		public bool ShowExport1C { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Экспорт в Xml"),
		 WebDisplayName("Поля для копирования в документы пакета"),
		 WebDescription("Поля для копирования в документы пакета")]
		public string CopyFields1C { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Экспорт в Xml"),
		 WebDisplayName("Необходимые поля для выгрузки"),
		 WebDescription("Необходимые поля для выгрузки")]
		public string RequieredFields1C { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Экспорт в Xml"),
		 WebDisplayName("Статусы в которых отображать"),
		 WebDescription("Статусы в которых отображать")]
		public string Show1CStatuses { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Экспорт в Xml"),
		 WebDisplayName("Группа, имеющая право на использование"),
		 WebDescription("Группа, имеющая право на использование")]
		public string XMLField { get; set; }

		#endregion

		#region Печатная форма

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue("Новый"),
		 Category("Печатная форма"),
		 WebDisplayName("Статусы для экспорта"),
		 WebDescription("Показывать кнопку экспорта формы только для документов в данных статусах (перечисление через ;)")]
		public string ShowApprovalListInWordStatuses { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(false),
		 Category("Печатная форма"),
		 WebDisplayName("Отображать печатную форму"),
		 WebDescription("Отображать печатную форму (броузер, PDF или Word)")]
		public bool ShowApprovalListInExternalApp { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue("Шаблоны документов"),
		 Category("Печатная форма"),
		 WebDisplayName("Список шаблонов"),
		 WebDescription("Список шаблонов")]
		public string TemplatesListName { get; set; }

		#endregion

		#region Связывание пакета

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(false),
		 Category("Связывание пакета"),
		 WebDisplayName("Показывать кнопку связывания"),
		 WebDescription("Показывать кнопку связывания")]
		public bool ShowLinkButton { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Связывание пакета"),
		 WebDisplayName("Cсылка на связывание"),
		 WebDescription("Cсылка на связывание")]
		public string LinkButtonURL { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Связывание пакета"),
		 WebDisplayName("Статусы в которых отображать"),
		 WebDescription("Статусы в которых отображать")]
		public string LinkButtonStatuses { get; set; }

		#endregion

		#region Маршрут согласования

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(false),
		 Category("Маршрут согласования"),
		 WebDisplayName("Показывать маршрут согласования"),
		 WebDescription("Показывать маршрут согласования")]
		public bool ShowApprovalRoute { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Маршрут согласования"),
		 WebDisplayName("Cсылка на маршрут согласования"),
		 WebDescription("Cсылка на маршрут согласования")]
		public string ApprovalRouteURL { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Маршрут согласования"),
		 WebDisplayName("Статусы в которых отображать"),
		 WebDescription("Статусы в которых отображать")]
		public string ApprovalRouteState { get; set; }

		#endregion

		#region Проверка дублей

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Проверка дубликатов"),
		 WebDisplayName("Проверять дубликаты"),
		 WebDescription("Выполнять ли проверку на наличие дубликатов карточек и документов")]
		public bool CheckTwins { get; set; }

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("Проверка дубликатов"),
		 WebDisplayName("Список полей"),
		 WebDescription("Список полей, по которым проводится поиск дубликатов")]
		public string TwinsSearchFields { get; set; }

		#endregion

		#region OnReWork

		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(false),
		 Category("OnRework"),
		 WebDisplayName("Отображать кнопку OnRework"),
		 WebDescription("Отображать кнопку OnRework")]
		public bool ShowOnRework { get; set; }



		[WebBrowsable(true),
		 Personalizable(PersonalizationScope.Shared),
		 DefaultValue(""),
		 Category("OnRework"),
		 WebDisplayName("Статусы рабочего процесса при которых отображается кнопка OnRework"),
		 WebDescription("Статусы рабочего процесса")]
		public string WorkflowStatusOnRework { get; set; }

		#endregion

		#endregion

		#region copycard
		
		public int CreateNewDocSet(SPWeb inWeb, string docSetName, string bCode, string listName, string contentTypeName,
			string subfolderName)
		{
			var itemId = 0;
			using (var site = new SPSite(inWeb.Url))
			{
				using (var web = site.OpenWeb())
				{
					var list = web.Lists[listName];
					var docSetCT = list.ContentTypes[contentTypeName];
					var props = new Hashtable();
					props.Add("BarCode", bCode);
					props.Add("IsDocumentSet", 1);
					//EnsureParentFolder(web, destinationPath, list);
					var incFolder = web.GetFolder(subfolderName);
					web.AllowUnsafeUpdates = true;
					web.Update();
					var docSet = DocumentSet.Create(incFolder, docSetName, docSetCT.Id, props, true);
					itemId = docSet.Item.ID;
				}
			}
			return itemId;
		}
		
		#endregion
	}
}