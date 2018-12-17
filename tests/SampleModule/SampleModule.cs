using System;
using System.Management.Automation;

namespace SampleModule
{
    [Cmdlet(VerbsCommon.Get, "Foo")]
    public class GetFooCmdlet : PSCmdlet
    {

    }
}
