﻿using DragonLens.Configs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;
using FixedUIScrollbar = Terraria.GameContent.UI.Elements.FixedUIScrollbar;

namespace DragonLens.Content.GUI
{
	internal abstract class Browser : DraggableUIState
	{
		private UIGrid options;
		private FixedUIScrollbar scrollBar;
		private SearchBar searchBar;

		public bool initialized;

		public abstract string Name { get; }

		public virtual string IconTexture => "DragonLens/Assets/Tools/TestTool";

		public override Rectangle DragBox => new((int)basePos.X, (int)basePos.Y, 500, 64);

		public override int InsertionIndex(List<GameInterfaceLayer> layers)
		{
			return layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
		}

		public abstract void PopulateGrid(UIGrid grid);

		public void AddButton(BrowserButton button)
		{
			//Might need to add positioning code here?
			options.Append(button);
		}

		public void SortGrid()
		{
			options.UpdateOrder();
		}

		public void FilterGrid(Func<BrowserButton, bool> filter)
		{
			PopulateGrid(options);
			options.Children.ToList().RemoveAll(n => !filter((BrowserButton)n));
		}

		public virtual void PostInitialize() { }

		public sealed override void SafeOnInitialize()
		{
			width = 500;
			height = 600;

			scrollBar = new(UserInterface);
			scrollBar.Width.Set(24, 0);
			scrollBar.Height.Set(480, 0);
			Append(scrollBar);

			options = new();
			options.Width.Set(460, 0);
			options.Height.Set(480, 0);
			options.SetScrollbar(scrollBar);
			Append(options);

			searchBar = new();
			searchBar.Width.Set(200, 0);
			searchBar.Height.Set(32, 0);
			Append(searchBar);

			PostInitialize();
		}

		public override void AdjustPositions(Vector2 newPos)
		{
			scrollBar.Left.Set(newPos.X + 464, 0);
			scrollBar.Top.Set(newPos.Y + 110, 0);

			options.Left.Set(newPos.X + 10, 0);
			options.Top.Set(newPos.Y + 110, 0);

			searchBar.Left.Set(newPos.X + 10, 0);
			searchBar.Top.Set(newPos.Y + 66, 0);
		}

		public void Refresh()
		{
			RemoveAllChildren();
			OnInitialize();
			PopulateGrid(options);
			SortGrid();
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			var target = new Rectangle((int)basePos.X, (int)basePos.Y, 500, 600);

			Helpers.GUIHelper.DrawBox(spriteBatch, target, ModContent.GetInstance<GUIConfig>().backgroundColor);

			Texture2D back = ModContent.Request<Texture2D>("DragonLens/Assets/GUI/Gradient").Value;
			var backTarget = new Rectangle((int)basePos.X + 8, (int)basePos.Y + 8, 400, 48);
			spriteBatch.Draw(back, backTarget, Color.Black * 0.5f);

			Texture2D gridBack = Terraria.GameContent.TextureAssets.MagicPixel.Value;
			var gridBackTarget = options.GetDimensions().ToRectangle();
			gridBackTarget.Inflate(4, 4);
			spriteBatch.Draw(gridBack, gridBackTarget, Color.Black * 0.25f);

			Texture2D icon = ModContent.Request<Texture2D>(IconTexture).Value;
			spriteBatch.Draw(icon, basePos + Vector2.One * 16, Color.White);

			Utils.DrawBorderStringBig(spriteBatch, Name, basePos + new Vector2(icon.Width + 24, 16), Color.White, 0.6f);

			base.Draw(spriteBatch);
		}
	}

	internal abstract class BrowserButton : UIElement
	{
		public abstract string Identifier { get; }

		public BrowserButton()
		{
			Width.Set(36, 0);
			Height.Set(36, 0);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			Helpers.GUIHelper.DrawBox(spriteBatch, GetDimensions().ToRectangle(), ModContent.GetInstance<GUIConfig>().buttonColor);

			base.Draw(spriteBatch);
		}
	}

	internal class SearchBar : UIElement
	{
		public string searchingFor = "";

		public override void Click(UIMouseEvent evt)
		{
			searchingFor = Main.GetInputText(searchingFor);
		}

		public override void Draw(SpriteBatch spriteBatch)
		{
			Helpers.GUIHelper.DrawBox(spriteBatch, GetDimensions().ToRectangle(), ModContent.GetInstance<GUIConfig>().buttonColor);

			Vector2 pos = GetDimensions().Position() + Vector2.One * 4;
			Utils.DrawBorderString(spriteBatch, searchingFor, pos, Color.White);
		}
	}
}
