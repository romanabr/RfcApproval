using SPMeta2.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSys.RFC.Data.Fields
{
	public class RFCFieldsCollection
	{
		public static List<FieldDefinition> RFCSiteFields = new List<FieldDefinition>
		{
			RFCFields.Description,
			RFCFields.FullDescription,
			RFCFields.EndDateFact,
			RFCFields.EndDatePlan,
			RFCFields.InteraptionFlag,
			RFCFields.Manager,
			RFCFields.StartDateFact,
			RFCFields.StartDatePlan,
			RFCFields.Status,
			
			RFCFields.KeType,
			RFCFields.KeMnemonica,
			RFCFields.KeToTypeLink,
			RFCFields.KeToKeLink,
			RFCFields.KeManager,
			RFCFields.KeParentLink,
			RFCFields.KeChildLink,

			RFCFields.RFCKeApprove,
			RFCFields.RFCKeApproveDate,
			RFCFields.RFCKeComment,

			RFCFields.Type,
			RFCFields.RFCBusinessRole,
			RFCFields.RFCUser,
			RFCFields.RFCUserType
		};
		
	}
}
