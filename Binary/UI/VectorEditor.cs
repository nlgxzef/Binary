﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using Binary.Properties;

using Nikki.Support.Shared.Class;

namespace Binary.UI
{
	public partial class VectorEditor : Form
	{
		private VectorVinyl Vector { get; }

		public VectorEditor(VectorVinyl vinyl)
		{
			this.InitializeComponent();
			this.ToggleTheme();
			this.Vector = vinyl;
			this.Text = $"{this.Vector.CollectionName} Editor";
			this.LoadTreeView();
			this.ToggleMenuStripControls(null);
		}

		#region Theme

		private void ToggleTheme()
		{
			// Renderers
			this.VectorMenuStrip.Renderer = new Theme.MenuStripRenderer();

			// Primary colors and controls
			this.BackColor = Theme.MainBackColor;
			this.ForeColor = Theme.MainForeColor;

			// Tree view
			this.VectorTreeView.BackColor = Theme.PrimBackColor;
			this.VectorTreeView.ForeColor = Theme.PrimForeColor;

			// Property grid
			this.VectorPropertyGrid.BackColor = Theme.PrimBackColor;
			this.VectorPropertyGrid.CategorySplitterColor = Theme.ButtonBackColor;
			this.VectorPropertyGrid.CategoryForeColor = Theme.TextBoxForeColor;
			this.VectorPropertyGrid.CommandsBackColor = Theme.PrimBackColor;
			this.VectorPropertyGrid.CommandsForeColor = Theme.PrimForeColor;
			this.VectorPropertyGrid.CommandsBorderColor = Theme.PrimBackColor;
			this.VectorPropertyGrid.DisabledItemForeColor = Theme.LabelTextColor;
			this.VectorPropertyGrid.LineColor = Theme.ButtonBackColor;
			this.VectorPropertyGrid.SelectedItemWithFocusBackColor = Theme.FocusedBackColor;
			this.VectorPropertyGrid.SelectedItemWithFocusForeColor = Theme.FocusedForeColor;
			this.VectorPropertyGrid.ViewBorderColor = Theme.RegBorderColor;
			this.VectorPropertyGrid.ViewBackColor = Theme.PrimBackColor;
			this.VectorPropertyGrid.ViewForeColor = Theme.PrimForeColor;
			this.VectorPropertyGrid.HelpBackColor = Theme.PrimBackColor;
			this.VectorPropertyGrid.HelpForeColor = Theme.PrimForeColor;
			this.VectorPropertyGrid.HelpBorderColor = Theme.RegBorderColor;

			// Menu strip and menu items
			this.VectorMenuStrip.ForeColor = Theme.LabelTextColor;
			this.ImportSVGToolStripMenuItem.BackColor = Theme.MenuItemBackColor;
			this.ImportSVGToolStripMenuItem.ForeColor = Theme.MenuItemForeColor;
			this.ExportSVGToolStripMenuItem.BackColor = Theme.MenuItemBackColor;
			this.ExportSVGToolStripMenuItem.ForeColor = Theme.MenuItemForeColor;
			this.PreviewToolStripMenuItem.BackColor = Theme.MenuItemBackColor;
			this.PreviewToolStripMenuItem.ForeColor = Theme.MenuItemForeColor;
			this.AddPathSetToolStripMenuItem.BackColor = Theme.MenuItemBackColor;
			this.AddPathSetToolStripMenuItem.ForeColor = Theme.MenuItemForeColor;
			this.RemovePathSetToolStripMenuItem.BackColor = Theme.MenuItemBackColor;
			this.RemovePathSetToolStripMenuItem.ForeColor = Theme.MenuItemForeColor;
			this.MoveUpPathSetToolStripMenuItem.BackColor = Theme.MenuItemBackColor;
			this.MoveUpPathSetToolStripMenuItem.ForeColor = Theme.MenuItemForeColor;
			this.MoveDownPathSetToolStripMenuItem.BackColor = Theme.MenuItemBackColor;
			this.MoveDownPathSetToolStripMenuItem.ForeColor = Theme.MenuItemForeColor;
		}

		#endregion

		#region Methods

		private object GetSelectedObject(TreeNode node)
		{
			if (node is null || node.Level == 1)
			{

				return null;

			}
			else if (node.Level == 0)
			{

				return this.Vector.GetPathSet(node.Index);

			}
			else if (node.Level == 2)
			{

				if (node.Parent.Text == "PathDatas")
				{

					var set = this.Vector.GetPathSet(node.Parent.Parent.Index);
					return set.PathDatas[node.Index];

				}
				else if (node.Parent.Text == "PathPoints")
				{

					var set = this.Vector.GetPathSet(node.Parent.Parent.Index);
					return set.PathPoints[node.Index];

				}

			}

			return null;
		}

		private void LoadTreeView(string selected = null)
		{
			this.VectorTreeView.Nodes.Clear();
			this.VectorTreeView.BeginUpdate();
			var nodes = new TreeNode[this.Vector.NumberOfPaths];

			for (int i = 0; i < this.Vector.NumberOfPaths; ++i)
			{

				var set = this.Vector.GetPathSet(i);
				var setnode = new TreeNode($"PathSet{i}");
				nodes[i] = setnode;

			}

			this.VectorTreeView.Nodes.AddRange(nodes);
			this.VectorTreeView.EndUpdate();

			if (!String.IsNullOrEmpty(selected))
			{

				this.RecursiveNodeSelection(selected, this.VectorTreeView.Nodes);

			}
		}

		private void RecursiveNodeSelection(string path, TreeNodeCollection nodes)
		{
			foreach (TreeNode node in nodes)
			{

				if (node.FullPath == path)
				{

					this.VectorTreeView.SelectedNode = node;
					return;

				}
				else
				{

					this.RecursiveNodeSelection(path, node.Nodes);

				}

			}
		}

		private void ToggleMenuStripControls(TreeNode node)
		{
			this.ImportSVGToolStripMenuItem.Enabled = true;
			this.ExportSVGToolStripMenuItem.Enabled = true;
			this.PreviewToolStripMenuItem.Enabled = true;
			this.AddPathSetToolStripMenuItem.Enabled = true;

			if (node is null)
			{

				this.RemovePathSetToolStripMenuItem.Enabled = false;
				this.MoveUpPathSetToolStripMenuItem.Enabled = false;
				this.MoveDownPathSetToolStripMenuItem.Enabled = false;

			}
			else
			{

				this.MoveUpPathSetToolStripMenuItem.Enabled = true;
				this.RemovePathSetToolStripMenuItem.Enabled = true;
				this.MoveDownPathSetToolStripMenuItem.Enabled = true;

			}
		}

		#endregion

		private void ImportSVGToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void ExportSVGToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void PreviewToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Not implemented yet", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void AddPathSetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var count = this.Vector.NumberOfPaths;
			this.Vector.AddPathSet();
			this.VectorTreeView.Nodes.Add($"PathSet{count}");
		}

		private void RemovePathSetToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this.VectorTreeView.SelectedNode is null) return;
			this.VectorPropertyGrid.SelectedObject = null;
			this.Vector.RemovePathSet(this.VectorTreeView.SelectedNode.Index);
			this.VectorTreeView.Nodes.RemoveAt(this.VectorTreeView.SelectedNode.Index);
		}

		private void MoveUpPathSetToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void MoveDownPathSetToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		#region TreeView and Grid

		private void VectorTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
		{
			if (this.VectorTreeView.SelectedNode != null)
			{

				this.VectorTreeView.SelectedNode.ForeColor = this.VectorTreeView.ForeColor;

				e.Node.ForeColor = Configurations.Default.DarkTheme
					? Color.FromArgb(255, 230, 0)
					: Color.FromArgb(255, 20, 20);

			}
			else
			{

				e.Node.ForeColor = Configurations.Default.DarkTheme
					? Color.FromArgb(255, 230, 0)
					: Color.FromArgb(255, 90, 0);

			}
		}

		private void VectorTreeView_AfterSelect(object sender, TreeViewEventArgs e)
		{
			Console.WriteLine(e.Node?.FullPath);
			var selected = this.GetSelectedObject(e.Node);
			this.ToggleMenuStripControls(e.Node);
			this.VectorPropertyGrid.SelectedObject = selected;
		}

		private void VectorPropertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
		{
		}

		#endregion
	}
}