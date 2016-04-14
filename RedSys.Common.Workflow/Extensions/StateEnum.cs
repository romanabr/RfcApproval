using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedSys.Common.Workflow
{
    enum StateEnum
    {
        Start,
        Performed,
        Revision,
        Stop,
        End
    }

    public enum StageType
    {
        Auto,
        Manual,
        Code
    }

    public enum AgreementType
    {
        Parallel,
        Successive
    }

    /*public enum RoleKindScheme
    {
        MVZ,
        STATE,
        LEGAL,
        MVZ_STATE,
        MVZ_LEGAL,
        STATE
    }*/

    public enum RoleKindScheme
    {
        MVZ = 1,
        STATE = 5,
        LEGAL = 10,
        BRANCH = 30
    }

    public enum Operation
    {
        BeginsWith,
        Contains,
        Eq,
        Geq,
        Gt,
        IsNotNull,
        IsNull,
        Leq,
        Lt,
        Neq
    }

    public enum FieldType
    {
        Int32,
        Decimal,
        Double,
        DateTime,
        String,
        Boolean
    }
}