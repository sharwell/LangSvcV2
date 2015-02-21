namespace Tvl.VisualStudio.Language.Java.Project.PropertyPages
{
    using System;
    using System.Runtime.InteropServices;
    using Tvl.VisualStudio.Shell;
    using VSCOMPONENTTYPE = Microsoft.VisualStudio.Shell.Interop.VSCOMPONENTTYPE;

    [ComVisible(true)]
    [Guid(JavaProjectConstants.MavenComponentSelectorGuidString)]
    internal partial class MavenComponentSelector : ComponentSelectorControl
    {
        public MavenComponentSelector()
        {
            InitializeComponent();
        }

        protected override bool CanSelectItems
        {
            get
            {
                return true;
            }
        }

        protected override void InitializeItems()
        {
        }

        protected override void ClearSelection()
        {
        }

        protected override void SetSelectionMode(bool multiSelect)
        {
        }

        protected override ComponentSelectorData[] GetSelection()
        {
            ComponentSelectorData data = new ComponentSelectorData
            {
                ComponentType = VSCOMPONENTTYPE.VSCOMPONENTTYPE_Custom,
                Title = string.Format("maven:{0}:{1}:{2}:{3}", txtGroupId.Text, txtArtifactId.Text, txtVersion.Text, txtClassifier.Text)
            };

            return new[] { data };
        }
    }
}
