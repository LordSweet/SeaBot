using SeaBotCore;
using SeaBotCore.Data.Materials;
using SeaBotCore.Utils;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace SeaBotGUI.Forms
{
    public partial class SelectUpgradableItems : Form
    {

        private readonly List<SelectedUpgradableItem> locallist = new List<SelectedUpgradableItem>();

        public SelectUpgradableItems()
        {
            this.InitializeComponent();

            if (Core.LocalPlayer != null && Core.LocalPlayer.Upgradeables != null)
            {
                foreach (var upgradeable in Core.LocalPlayer.Upgradeables)
                {
                    var mat = upgradeable.Definition.MaterialId;
                    if (Core.Config.upgitems.Contains(mat))
                    {
                        this.locallist.Add(new SelectedUpgradableItem { Id = mat, Selected = true });
                    }
                    else
                    {
                        if (!locallist.Any(n => n.Id == mat))
                        {
                            this.locallist.Add(new SelectedUpgradableItem { Id = mat, Selected = false });
                        }
                    }
                }
            }

            ((ListBox)this.checkedListBox1).DataSource = this.locallist;
            ((ListBox)this.checkedListBox1).DisplayMember = "Name";
            ((ListBox)this.checkedListBox1).ValueMember = "Selected";
            for (var i = 0; i < this.checkedListBox1.Items.Count; i++)
            {
                var obj = (SelectedUpgradableItem)this.checkedListBox1.Items[i];
                this.checkedListBox1.SetItemChecked(i, obj.Selected);
            }

            this.checkedListBox1.ItemCheck += this.CheckedListBox1_ItemCheck;
        }

        private void CheckedListBox1_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var obj = (SelectedUpgradableItem)this.checkedListBox1.Items[e.Index];
            if (obj != null)
            {
                if (e.NewValue == CheckState.Checked)
                {
                    if (!Core.Config.upgitems.Contains(obj.Id))
                    {
                        var snapshot = Core.Config.upgitems;
                        snapshot.Add(obj.Id);
                        Core.Config.upgitems = snapshot;
                    }
                }
                else
                {
                    if (Core.Config.upgitems.Contains(obj.Id))
                    {
                        var snapshot = Core.Config.upgitems;
                        snapshot.Remove(obj.Id);
                        Core.Config.upgitems = snapshot;
                    }
                }
            }
        }
    }

   
        public class SelectedUpgradableItem
        {
            public int Id { get; set; }

            public string Name => MaterialDB.GetLocalizedName(this.Id);

            public bool Selected { get; set; }
        }
    
}

