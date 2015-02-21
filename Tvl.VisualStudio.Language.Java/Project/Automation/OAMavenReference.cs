namespace Tvl.VisualStudio.Language.Java.Project.Automation
{
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Project.Automation;

    [ComVisible(true)]
    public class OAMavenReference : OAReferenceBase<MavenReferenceNode>
    {
        public OAMavenReference(MavenReferenceNode mavenReference)
            : base(mavenReference)
        {
        }

        public override string Name
        {
            get
            {
                return BaseReferenceNode.Caption;
            }
        }

        public override string Culture
        {
            get
            {
                return string.Empty;
            }
        }
    }
}
