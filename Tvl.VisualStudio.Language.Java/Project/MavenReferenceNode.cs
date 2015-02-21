namespace Tvl.VisualStudio.Language.Java.Project
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.Contracts;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using Microsoft.VisualStudio.Project;
    using File = System.IO.File;
    using Path = System.IO.Path;

    [ComVisible(true)]
    public class MavenReferenceNode : ReferenceNode
    {
        private readonly string _projectRelativeFilePath;

        /// <summary>
        /// Holds the value passed to <see cref="MavenReferenceNode(ProjectNode, string, string, string, string)"/> so
        /// it can be used in <see cref="BindReferenceData"/> to create the project element. This field is not always
        /// set. See the implementation of <see cref="GroupId"/>.
        /// </summary>
        private readonly string _groupId;
        /// <summary>
        /// Holds the value passed to <see cref="MavenReferenceNode(ProjectNode, string, string, string, string)"/> so
        /// it can be used in <see cref="BindReferenceData"/> to create the project element. This field is not always
        /// set. See the implementation of <see cref="ArtifactId"/>.
        /// </summary>
        private readonly string _artifactId;
        /// <summary>
        /// Holds the value passed to <see cref="MavenReferenceNode(ProjectNode, string, string, string, string)"/> so
        /// it can be used in <see cref="BindReferenceData"/> to create the project element. This field is not always
        /// set. See the implementation of <see cref="Version"/>.
        /// </summary>
        private readonly string _version;
        /// <summary>
        /// Holds the value passed to <see cref="MavenReferenceNode(ProjectNode, string, string, string, string)"/> so
        /// it can be used in <see cref="BindReferenceData"/> to create the project element. This field is not always
        /// set. See the implementation of <see cref="Classifier"/>.
        /// </summary>
        private readonly string _classifier;

        private Automation.OAMavenReference _mavenReference;

        public MavenReferenceNode(ProjectNode root, ProjectElement element)
            : base(root, element)
        {
            Contract.Requires<ArgumentNullException>(root != null, "root");
            Contract.Requires<ArgumentNullException>(element != null, "element");

            _projectRelativeFilePath = element.Item.EvaluatedInclude;
            ProjectManager.ItemIdMap.UpdateCanonicalName(this);
        }

        public MavenReferenceNode(ProjectNode root, string groupId, string artifactId, string version, string classifier)
            : base(root)
        {
            Contract.Requires<ArgumentNullException>(root != null, "root");
            Contract.Requires<ArgumentNullException>(groupId != null, "groupId");
            Contract.Requires<ArgumentNullException>(artifactId != null, "artifactId");
            Contract.Requires<ArgumentNullException>(version != null, "version");
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(groupId));
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(artifactId));
            Contract.Requires<ArgumentException>(!string.IsNullOrEmpty(version));

            _groupId = groupId;
            _artifactId = artifactId;
            _version = version;
            _classifier = classifier;

            string classifierSuffix = string.IsNullOrEmpty(classifier) ? string.Empty : ("-" + classifier);

            // packages/{groupId}.{artifactId}.{version}/{artifactId}-{version}{classifierSuffix}.jar
            string artifactFolder = string.Format("{0}.{1}.{2}", groupId, artifactId, version);
            string artifactFile = string.Format("{0}-{1}{2}.jar", artifactId, version, classifierSuffix);
            _projectRelativeFilePath = Path.Combine("packages", artifactFolder, artifactFile);
        }

        public string InstalledFilePath
        {
            get
            {
                return Path.Combine(ProjectManager.ProjectFolder, ProjectRelativeFilePath);
            }
        }

        public override string Url
        {
            get
            {
                return ProjectRelativeFilePath;
            }
        }

        public override bool CanCacheCanonicalName
        {
            get
            {
                return !string.IsNullOrEmpty(ProjectRelativeFilePath);
            }
        }

        public override string Caption
        {
            get
            {
                return Path.GetFileNameWithoutExtension(ProjectRelativeFilePath);
            }
        }

        public override object Object
        {
            get
            {
                if (_mavenReference == null)
                    _mavenReference = new Automation.OAMavenReference(this);

                return _mavenReference;
            }
        }

        public string GroupId
        {
            get
            {
                if (_groupId != null)
                    return _groupId;

                ProjectElement element = ItemNode;
                if (element == null)
                    return null;

                return element.GetMetadata(JavaProjectFileConstants.GroupId);
            }
        }

        public string ArtifactId
        {
            get
            {
                if (_artifactId != null)
                    return _artifactId;

                ProjectElement element = ItemNode;
                if (element == null)
                    return null;

                return element.GetMetadata(JavaProjectFileConstants.ArtifactId);
            }
        }

        public string Version
        {
            get
            {
                if (_version != null)
                    return _version;

                ProjectElement element = ItemNode;
                if (element == null)
                    return null;

                return element.GetMetadata(JavaProjectFileConstants.Version);
            }
        }

        public string Classifier
        {
            get
            {
                if (_classifier != null)
                    return _classifier;

                ProjectElement element = ItemNode;
                if (element == null)
                    return null;

                return element.GetMetadata(JavaProjectFileConstants.Classifier);
            }
        }

        private string ProjectRelativeFilePath
        {
            get
            {
                return _projectRelativeFilePath;
            }
        }

        protected override NodeProperties CreatePropertiesObject()
        {
            return new MavenReferenceProperties(this);
        }

        protected override void BindReferenceData()
        {
            if (ItemNode == null || ItemNode.Item == null)
            {
                ProjectElement element = new ProjectElement(ProjectManager, ProjectRelativeFilePath, JavaProjectFileConstants.MavenReference);

                // Set the basic information about this reference
                element.SetMetadata(JavaProjectFileConstants.IncludeInBuild, true.ToString());
                element.SetMetadata(JavaProjectFileConstants.GroupId, _groupId);
                element.SetMetadata(JavaProjectFileConstants.ArtifactId, _artifactId);
                element.SetMetadata(JavaProjectFileConstants.Version, _version);
                if (!string.IsNullOrEmpty(_classifier))
                    element.SetMetadata(JavaProjectFileConstants.Classifier, _classifier);

                ItemNode = element;
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        public override bool IsAlreadyAdded(out ReferenceNode existingEquivalentNode)
        {
            string fullPath = Path.GetFullPath(InstalledFilePath).Replace('\\', '/');

            ReferenceContainerNode referencesFolder = this.ProjectManager.FindChild(ReferenceContainerNode.ReferencesNodeVirtualName) as ReferenceContainerNode;
            for (HierarchyNode node = referencesFolder.FirstChild; node != null; node = node.NextSibling)
            {
                MavenReferenceNode referenceNode = node as MavenReferenceNode;
                if (referenceNode != null)
                {
                    string otherFullPath = Path.GetFullPath(referenceNode.InstalledFilePath).Replace('\\', '/');
                    if (string.Equals(fullPath, otherFullPath, StringComparison.OrdinalIgnoreCase))
                    {
                        existingEquivalentNode = referenceNode;
                        return true;
                    }
                }
            }

            existingEquivalentNode = null;
            return false;
        }

        protected override bool CanShowDefaultIcon()
        {
            return File.Exists(InstalledFilePath);
        }

        protected override void ResolveReference()
        {
        }
    }
}
