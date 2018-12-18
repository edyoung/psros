using System.Management.Automation;

namespace SampleModule
{
    [Cmdlet(VerbsCommon.Get, "Foo")]
    public class GetFooCmdlet : PSCmdlet
    {

    }

    [Cmdlet("BadVerb", "Foo")]
    public class GetFooCmdlet2 : PSCmdlet
    {

    }
}
