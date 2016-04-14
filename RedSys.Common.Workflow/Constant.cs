using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ensol.Common.Workflow
{
    public static class Constant
    {
        public const string QUERYBYDOCTYPE = @"<Where><And><Eq><FieldRef ID='{0}' /><Value Type='Text'>{1}</Value></Eq><Eq><FieldRef ID='{2}' LookupId='TRUE' /><Value Type='Lookup'>{3}</Value></Eq></And></Where>";
        
        public const string SEQUENTIALSTAGE = "Последовательный";
        public const string PARALLELSTAGE = "Параллельный";
        public const string AUTOSTAGE = "Автоматический";
        public const string MANUALSTAGE = "Ручной";

        public static Guid StageNumber = new Guid("0FDF16DA-06E1-4D53-8B53-1E90C4200E3A");
        public static Guid StageType = new Guid("3F83720E-FD0F-486C-9FE9-AE5A49783118");
        public static Guid Waiting = new Guid("43527D0C-0F22-414B-9A02-7101A83F30A8");
        public static Guid DocumentType_Ref = new Guid("1C40FBA5-F6FE-4B9A-9FCD-7E01EF187C3D");
        public static Guid DocumentKind_Ref = new Guid("935CDC05-DDBB-48D1-A432-DAE729285EC7");
        public static string DocumentKind = "Вид документа";
        public static Guid StageName = new Guid("019772F0-BF2D-48E6-8347-D4D00EE5DCCC");
        public static Guid Role_Ref = new Guid("D3218AFD-488A-4C37-92EA-8E00C1097DC0");
        public static string Role_Name = "Роль";
        public static string CopyFields = "Поля для переноса в задачу";
        public static Guid Duration = new Guid("757A3CCA-A4A0-49AD-B313-7EF4E45A51A4");
        public static Guid ForcedAgreement = new Guid("8A90692C-454F-4BCF-97A6-A921772DE31E");
        public static Guid TypeAgreement = new Guid("7635B5C0-D8CD-462E-A9BA-8FED98AE06CD");
        public static Guid RepeatedAgreement = new Guid("E59BDD9E-8FE6-4330-A034-A83EBCAC75FF");
        public static Guid StepNextGood = new Guid("61DFC3AE-0CAD-4848-8E90-FFE2A66C3D8F");
        public static Guid StepNextBad = new Guid("A16CB1E6-A926-4013-AD2A-328AE77CC068");
        public static Guid ListOfAgreement = new Guid("F1AA78C9-F475-4F9A-BA88-9E372B3D3419");
        public static Guid ContentOfTask = new Guid("011507FE-900C-4078-9419-5AB41BD7DE27");
        public static string TypeOfTask_str = "Тип задания";
        public static Guid TypeOfTask_Guid = new Guid("ED7EBB1F-3839-4A4A-98D8-72300D63E827");
        public static Guid CardField = new Guid("03AC2810-CB4E-4E1E-AD94-6F9F1CDDC4D6");
        public static Guid ComparePatternText = new Guid("4F99F18E-6990-4F89-900E-AA4B325E594E");
        public static Guid ComparePatternInt = new Guid("6B58BEA3-4BA6-4EDD-8334-CD5DDF77203B");
        public static Guid ComparePatternFloat = new Guid("E460B058-0D37-4F1A-AB78-EB587F4AEF73");
        public static Guid ComparePatternCurrent = new Guid("9C635906-1707-4B5C-AB09-0310F7E32367");
        public static Guid ComparePatternBool = new Guid("E96F23FE-FAB3-4D06-8A13-119AA01925C5");
        public static Guid ComparePatternDate = new Guid("DA7668D3-5179-4D64-8822-EBA3E375C6D6");
        public static Guid ComparePattern = new Guid("0647CA3A-E2D2-4C12-8A9B-41CB4F22567B");
        public static Guid FieldType = new Guid("7DCED563-6F08-430E-80D6-C3869777E64D");
        public static Guid Operation = new Guid("42A3FABD-FDCC-4946-8E0E-19522827947C");
        public static Guid DocTypeGuid = new Guid("D170BE8A-019C-45EF-B2AA-ABFA4DF96B62");
        public static Guid DocTypeGuid_Ref = new Guid("16f3f28e-fed5-49c3-b777-a6ebec050773");

        public static Guid PersonId = new Guid("13ADBFAB-25CA-4A0E-8591-804A5C18B12C");
        public static Guid AD = new Guid("E22F4A02-4248-4CFF-87B0-4DA63473880E");
        public static Guid PersonPosition = new Guid("5543C071-1E5A-4A44-BB5A-3E63E0068A0E");
        public static Guid RoleKind = new Guid("D5B9B028-E351-480C-B030-98BBFD89F4F0");
        public static Guid MVZ_Ref = new Guid("7678307E-2084-4306-AAAD-91D38AD01F65");
        public static Guid StateOfCost_Ref = new Guid("A6D35318-6850-4F19-B3A3-0F93AD837109");
        public static Guid LegalPerson_Ref = new Guid("6FAA08FE-B381-4EF7-90ED-049A0AD322D6");
        public static Guid Branch_Ref = new Guid("E4F18F12-6FD5-46A4-810F-DBBC0811F166");
        public static Guid MVZ = new Guid("03D09C1E-D7C1-4A5E-88FA-2BF23263A861");
        public static Guid StateofCost = new Guid("52739DE5-3F9D-453E-97F4-72DEE3579D8D");
        public static Guid LegalPerson = new Guid("0028F9D8-6075-451E-994D-E8AC38A10631");
        public static Guid Approval = new Guid("BD76650F-2E1F-4DD7-9EF8-3E2D0DF9E0A0");


        public static Guid StageGuid = new Guid("F0D9617F-67A7-4BD1-AED3-7256DE77E99F");
        public static Guid WFHistoryGuid = new Guid("FEFE8077-B6FD-4155-9980-B5D0B1FC2FAF");
        public static Guid WFXMLHistoryGuid = new Guid("91228F94-32D9-4D5C-A6CA-E426D5B26E5F");
        public static Guid EmptyUserNameGuid = new Guid("E487BCA2-738C-4B3D-9277-56BEDE271330");
        public static Guid TaskCommentGuid = new Guid("54E6DD1C-5AD5-453F-8D15-DD7F1DA46D9D");

        public static Guid AssignToUsers = new Guid("6F6D142F-914B-49CE-9DCF-7764B3F1CE04");
        public static Guid IsDefaultUser = new Guid("573B675A-51AA-490F-BF09-208C4708E095");

        public const string StageListUrl = "Lists/WFPropertyesLI";
        public const string RoleUrl = "Lists/RoleLI";
        public const string RoleDistributionUrl = "Lists/RolePropertyesLI";
        public const string DocumentTypeUrl = "Lists/DocumentTypeLI";
        public const string DocumentKindUrl = "Lists/DocumentKindList";

        public const string MVZTEXT = "МВЗ";
        public const string STATEOFCOSTTEXT = "Статья затрат";
        public const string LEGALPERSONTEXT = "Юридическое лицо";
        public const string BRANCHTEXT = "Филиал";

        public const string CALENDARURL = "/Lists/Calendar";
        public const string SYNCERRORLIST  = "/Lists/SyncError";
    }
}
