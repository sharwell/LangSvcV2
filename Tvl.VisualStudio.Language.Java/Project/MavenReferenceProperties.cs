namespace Tvl.VisualStudio.Language.Java.Project
{
    using System.ComponentModel;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.Project;

    [ComVisible(true)]
    public class MavenReferenceProperties : ReferenceNodeProperties
    {
        public MavenReferenceProperties(MavenReferenceNode node)
            : base(node)
        {
        }

        [AutomationBrowsable(false)]
        [Browsable(false)]
        public new MavenReferenceNode Node
        {
            get
            {
                return (MavenReferenceNode)base.Node;
            }
        }

        [Category("Maven")]
        [DisplayName("Group ID")]
        public string GroupId
        {
            get
            {
                return Node.GroupId;
            }
        }

        [Category("Maven")]
        [DisplayName("Artifact ID")]
        public string ArtifactId
        {
            get
            {
                return Node.ArtifactId;
            }
        }

        [Category("Maven")]
        [DisplayName("Version")]
        public string Version
        {
            get
            {
                return Node.Version;
            }
        }

        [Category("Maven")]
        [DisplayName("Classifier")]
        public string Classifier
        {
            get
            {
                return Node.Classifier ?? string.Empty;
            }
        }
    }
}
