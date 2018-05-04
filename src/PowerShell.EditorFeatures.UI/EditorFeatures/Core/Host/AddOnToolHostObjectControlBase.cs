using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Markup;
using Microsoft.PowerShell.Host.ISE;

namespace PowerShell.EditorFeatures.Core.Host
{
    [ContentProperty(nameof(Children))]
    public class AddOnToolHostObjectControlBase : AbstractAddOnToolHostObjectControl
    {

        public override ObjectModelRoot HostObject { get; set; }

        /// <summary>
        /// Gets the Children Property of this BarStaticItem.
        /// </summary>
        public ObservableCollection<UIElement> Children { get; private set; }


        protected void RegisterChildrenObservation()
        {
            Children = new ObservableCollection<UIElement>();
            Children.CollectionChanged += OnCollectionChanged;
        }

        protected virtual void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }
    }
}
