﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using DPoint = System.Drawing.Point;

using TShockAPI;

namespace Terraria.Plugins.Common {
  public class TerrariaItems {
    #region [Methods: IsValidType, IsCraftableType, IsEquipableType, IsFlailType, IsSpearType, IsBoomerangType, IsChainsawType, IsDrillType, IsHookType, IsCoinType, GetItemTypeFromBlockType, GetItemTypeFromWallType]
    public bool IsValidType(int itemType) {
      return (itemType >= TerrariaUtils.ItemType_Min && itemType <= TerrariaUtils.ItemType_Max);
    }

    private static bool[] craftableItemTypes;
    public bool IsCraftableType(ItemType itemType) {
      if ((int)itemType < TerrariaUtils.ItemType_Min || (int)itemType > TerrariaUtils.ItemType_Max)
        throw new ArgumentException(string.Format("The given item type {0} is invalid.", itemType), "itemType");

      if (TerrariaItems.craftableItemTypes == null) {
        TerrariaItems.craftableItemTypes = new bool[TerrariaUtils.ItemType_Max + 1 + Math.Abs(TerrariaUtils.ItemType_Min)];

        for (int i = 0; i < Main.recipe.Length; i++) {
          Recipe recipe = Main.recipe[i];
          if (recipe == null)
            continue;

          int index = recipe.createItem.netID;
          if (index < 0)
            index += TerrariaUtils.ItemType_Max;

          TerrariaItems.craftableItemTypes[index] = true;
        }
      }
      
      {
        int index = (int)itemType;
        if (index < 0)
          index += TerrariaUtils.ItemType_Max;

        return TerrariaItems.craftableItemTypes[index];
      }
    }

    private static bool[] equipableItemTypes;
    public bool IsEquipableType(ItemType itemType) {
      if ((int)itemType < TerrariaUtils.ItemType_Min || (int)itemType > TerrariaUtils.ItemType_Max)
        throw new ArgumentException(string.Format("The given item type {0} is invalid.", itemType), "itemType");
      
      if (TerrariaItems.equipableItemTypes == null) {
        TerrariaItems.equipableItemTypes = new bool[TerrariaUtils.ItemType_Max + 1 + Math.Abs(TerrariaUtils.ItemType_Min)];

        for (int i = TerrariaUtils.ItemType_Min; i < TerrariaUtils.ItemType_Max + 1; i++) {
          Item dummyItem = new Item();
          dummyItem.netDefaults(i);
          if (
            dummyItem.name != null && (
              dummyItem.headSlot != -1 || dummyItem.bodySlot != -1 || dummyItem.legSlot != -1 || dummyItem.accessory
            )
          ) {
            int index = i;
            if (index < 0)
              index += TerrariaUtils.ItemType_Max;

            TerrariaItems.equipableItemTypes[index] = true;
          }
        }
      }

      {
        int index = (int)itemType;
        if (index < 0)
          index += TerrariaUtils.ItemType_Max;

        return TerrariaItems.equipableItemTypes[index];
      }
    }

    private static List<ItemType> ammoTypes;
    public bool IsAmmoType(ItemType itemType) {
      if ((int)itemType < TerrariaUtils.ItemType_Min || (int)itemType > TerrariaUtils.ItemType_Max)
        throw new ArgumentException(string.Format("The given item type {0} is invalid.", itemType), "itemType");
      
      if (TerrariaItems.ammoTypes == null) {
        TerrariaItems.ammoTypes = new List<ItemType>(20);

        for (int i = TerrariaUtils.ItemType_Min; i < TerrariaUtils.ItemType_Max + 1; i++) {
          Item dummyItem = new Item();
          dummyItem.netDefaults(i);
          if (dummyItem.name != null && dummyItem.ammo > 0)
            TerrariaItems.ammoTypes.Add((ItemType)i);
        }
      }

      return TerrariaItems.ammoTypes.Contains(itemType);
    }

    private static List<ItemType> weaponTypes;
    public bool IsWeaponType(ItemType itemType) {
      if ((int)itemType < TerrariaUtils.ItemType_Min || (int)itemType > TerrariaUtils.ItemType_Max)
        throw new ArgumentException(string.Format("The given item type {0} is invalid.", itemType), "itemType");
      
      if (TerrariaItems.weaponTypes == null) {
        TerrariaItems.weaponTypes = new List<ItemType>(20);

        for (int i = TerrariaUtils.ItemType_Min; i < TerrariaUtils.ItemType_Max + 1; i++) {
          Item dummyItem = new Item();
          dummyItem.netDefaults(i);
          if (dummyItem.name != null && dummyItem.damage > 0)
            TerrariaItems.weaponTypes.Add((ItemType)i);
        }
      }

      return TerrariaItems.weaponTypes.Contains(itemType);
    }

    private static List<ItemType> accessoryTypes;
    public bool IsAccessoryType(ItemType itemType) {
      if ((int)itemType < TerrariaUtils.ItemType_Min || (int)itemType > TerrariaUtils.ItemType_Max)
        throw new ArgumentException(string.Format("The given item type {0} is invalid.", itemType), "itemType");
      
      if (TerrariaItems.accessoryTypes == null) {
        TerrariaItems.accessoryTypes = new List<ItemType>(20);

        for (int i = TerrariaUtils.ItemType_Min; i < TerrariaUtils.ItemType_Max + 1; i++) {
          Item dummyItem = new Item();
          dummyItem.netDefaults(i);
          if (dummyItem.name != null && dummyItem.accessory)
            TerrariaItems.accessoryTypes.Add((ItemType)i);
        }
      }

      return TerrariaItems.accessoryTypes.Contains(itemType);
    }

    private static List<ItemType> vanityTypes;
    public bool IsVanityType(ItemType itemType) {
      if ((int)itemType < TerrariaUtils.ItemType_Min || (int)itemType > TerrariaUtils.ItemType_Max)
        throw new ArgumentException(string.Format("The given item type {0} is invalid.", itemType), "itemType");
      
      if (TerrariaItems.vanityTypes == null) {
        TerrariaItems.vanityTypes = new List<ItemType>(20);

        for (int i = TerrariaUtils.ItemType_Min; i < TerrariaUtils.ItemType_Max + 1; i++) {
          Item dummyItem = new Item();
          dummyItem.netDefaults(i);
          if (dummyItem.name != null && dummyItem.vanity)
            TerrariaItems.vanityTypes.Add((ItemType)i);
        }
      }

      return TerrariaItems.vanityTypes.Contains(itemType);
    }

    private static ItemType[][] blockTypesItemTypes;
    public ItemType GetItemTypeFromBlockType(BlockType blockType, int objectStyle = 0) {
      if ((int)blockType < TerrariaUtils.BlockType_Min || (int)blockType > TerrariaUtils.BlockType_Max)
        throw new ArgumentException(string.Format("The given block type {0} is invalid.", blockType), "blockType");

      if (TerrariaItems.blockTypesItemTypes == null) {
        TerrariaItems.blockTypesItemTypes = new ItemType[TerrariaUtils.ItemType_Max + 1][];

        for (int i = TerrariaUtils.ItemType_Min; i < TerrariaUtils.ItemType_Max + 1; i++) {
          Item dummyItem = new Item();
          dummyItem.netDefaults(i);
          if (dummyItem.name != null && dummyItem.createTile != -1) {
            ItemType[] styleArray = TerrariaItems.blockTypesItemTypes[dummyItem.createTile];
            ItemType[] newStyleArray;

            if (styleArray != null) {
              newStyleArray = new ItemType[Math.Max(dummyItem.placeStyle + 1, styleArray.Length)];
              styleArray.CopyTo(newStyleArray, 0);
            } else {
              newStyleArray = new ItemType[dummyItem.placeStyle + 1];
            }

            newStyleArray[dummyItem.placeStyle] = (ItemType)i;
            TerrariaItems.blockTypesItemTypes[dummyItem.createTile] = newStyleArray;
          }
        }
      }

      {
        if (blockType == BlockType.Mannequin)
          return ItemType.Mannequin;

        ItemType[] styleArray = TerrariaItems.blockTypesItemTypes[(int)blockType];
        if (objectStyle >= styleArray.Length)
          throw new ArgumentException(string.Format("There is no item type for block \"{0}\" with object style {1}", blockType, objectStyle));

        return styleArray[objectStyle];
      }
    }

    private static ItemType[] wallTypesItemTypes;
    public ItemType GetItemTypeFromWallType(WallType wallType) {
      if ((int)wallType < TerrariaUtils.WallType_Min || (int)wallType > TerrariaUtils.WallType_Max)
        throw new ArgumentException(string.Format("The given item type {0} is invalid.", wallType), "wallType");

      if (TerrariaItems.wallTypesItemTypes == null) {
        TerrariaItems.wallTypesItemTypes = new ItemType[TerrariaUtils.WallType_Max + 1];

        for (int i = TerrariaUtils.ItemType_Min; i < TerrariaUtils.WallType_Max + 1; i++) {
          Item dummyItem = new Item();
          dummyItem.netDefaults(i);
          if (dummyItem.name != null && dummyItem.createWall != -1)
            TerrariaItems.wallTypesItemTypes[dummyItem.createWall] = (ItemType)i;
        }
      }

      return TerrariaItems.wallTypesItemTypes[(int)wallType];
    }

    public bool IsFlailType(ItemType itemType) {
      switch (itemType) {
        case ItemType.BallOHurt:
        case ItemType.BlueMoon:
        case ItemType.Harpoon:
        case ItemType.Sunfury:
        case ItemType.DaoofPow:
          return true;
      }

      return false;
    }

    public bool IsSpearType(ItemType itemType) {
      switch (itemType) {
        case ItemType.Spear:
        case ItemType.Trident:
        case ItemType.DarkLance:
        case ItemType.CobaltNaginata:
        case ItemType.MythrilHalberd:
        case ItemType.AdamantiteGlaive:
        case ItemType.Gungnir:
          return true;
      }

      return false;
    }

    public bool IsBoomerangType(ItemType itemType) {
      switch (itemType) {
        case ItemType.WoodenBoomerang:
        case ItemType.EnchantedBoomerang:
        case ItemType.ThornChakram:
        case ItemType.Flamarang:
        case ItemType.LightDisc:
          return true;
      }

      return false;
    }

    public bool IsChainsawType(ItemType itemType) {
      switch (itemType) {
        case ItemType.CobaltChainsaw:
        case ItemType.MythrilChainsaw:
        case ItemType.AdamantiteChainsaw:
          return true;
      }

      return false;
    }

    public bool IsDrillType(ItemType itemType) {
      switch (itemType) {
        case ItemType.CobaltDrill:
        case ItemType.MythrilDrill:
        case ItemType.AdamantiteDrill:
        case ItemType.Hamdrax:
          return true;
      }

      return false;
    }

    public bool IsHookType(ItemType itemType) {
      switch (itemType) {
        case ItemType.GrapplingHook:
        case ItemType.IvyWhip:
        case ItemType.DualHook:
          return true;
      }

      return false;
    }

    public bool IsArrowType(ItemType itemType) {
      switch (itemType) {
        case ItemType.WoodenArrow:
        case ItemType.FlamingArrow:
        case ItemType.UnholyArrow:
        case ItemType.JestersArrow:
        case ItemType.HellfireArrow:
        case ItemType.HolyArrow:
        case ItemType.CursedArrow:
          return true;
      }

      return false;
    }

    public bool IsCoinType(ItemType itemType) {
      switch (itemType) {
        case ItemType.CopperCoin:
        case ItemType.SilverCoin:
        case ItemType.GoldCoin:
        case ItemType.PlatinumCoin:
          return true;
      }

      return false;
    }
    #endregion

    #region [Methods: GetItemName, GetItemRepresentativeString]
    public string GetItemName(ItemData itemData, bool includePrefix = false) {
      if ((itemData.Prefix != ItemPrefix.None && includePrefix) || itemData.Type < 0) {
        Item dummyItem = new Item();
        dummyItem.netDefaults((int)itemData.Type);
        dummyItem.prefix = (byte)itemData.Prefix;

        return dummyItem.AffixName();
      }

      return Main.itemName[(int)itemData.Type];
    }

    public string GetItemName(ItemType itemType) {
      return this.GetItemName(new ItemData(ItemPrefix.None, itemType, 1));
    }

    public string GetItemRepresentativeString(ItemData itemData) {
      string format = "{0}";
      if (itemData.StackSize > 1)
        format = "{0} ({1})";

      return string.Format(format, this.GetItemName(itemData, true), itemData.StackSize);
    }
    #endregion

    #region [Method: CreateNew]
    public void CreateNew(TSPlayer forPlayer, DPoint location, ItemData itemData) {
      int itemIndex = Item.NewItem(
        location.X, location.Y, 0, 0, (int)itemData.Type, itemData.StackSize, true, (int)itemData.Prefix
      );

      forPlayer.SendData(PacketTypes.ItemDrop, string.Empty, itemIndex);
    }
    #endregion

    #region [Methods: EnumerateItemsInRect, EnumerateItemsAroundPoint]
    public IEnumerable<Item> EnumerateItemsInRect(Rectangle rect) {
      int areaL = rect.Left - (rect.Width / 2);
      int areaT = rect.Top - (rect.Height / 2);
      int areaR = rect.Left + (rect.Width / 2);
      int areaB = rect.Top + (rect.Height / 2);

      for (int i = 0; i < 200; i++) {
        Item item = Main.item[i];

        if (
          item.active &&
          item.position.X > areaL && item.position.X < areaR &&
          item.position.Y > areaT && item.position.Y < areaB
        ) {
          yield return item;
        }
      }
    }

    public IEnumerable<Item> EnumerateItemsAroundPoint(DPoint location, int radius) {
      for (int i = 0; i < 200; i++) {
        Item item = Main.item[i];

        if (
          item.active && 
          Math.Sqrt(Math.Pow(item.position.X - location.X, 2) + Math.Pow(item.position.Y - location.Y, 2)) <= radius
        )
          yield return item;
      }
    }
    #endregion

    #region [Methods: IsUniversalPrefix, IsCommonPrefix]
    public bool IsUniversalPrefix(ItemPrefix prefix) {
      switch (prefix) {
        case ItemPrefix.Keen:
        case ItemPrefix.Superior:
        case ItemPrefix.Forceful:
        case ItemPrefix.Broken:
        case ItemPrefix.Damaged:
        case ItemPrefix.Shoddy:
        case ItemPrefix.Hurtful:
        case ItemPrefix.Strong:
        case ItemPrefix.Unpleasant:
        case ItemPrefix.Weak:
        case ItemPrefix.Ruthless:
        case ItemPrefix.Godly:
        case ItemPrefix.Demonic:
        case ItemPrefix.Zealous:
          return true;
      }

      return false;
    }

    public bool IsCommonPrefix(ItemPrefix prefix) {
      switch (prefix) {
        case ItemPrefix.Quick:
        case ItemPrefix.Quick2:
        case ItemPrefix.Deadly:
        case ItemPrefix.Deadly2:
        case ItemPrefix.Agile:
        case ItemPrefix.Nimble:
        case ItemPrefix.Murderous:
        case ItemPrefix.Slow:
        case ItemPrefix.Sluggish:
        case ItemPrefix.Lazy:
        case ItemPrefix.Annoying:
        case ItemPrefix.Nasty:
          return true;
      }

      return false;
    }
    #endregion

    #region [Method: GetCoinItemValue]
    public int GetCoinItemValue(ItemData coinItem) {
      Contract.Requires<ArgumentException>(this.IsCoinType(coinItem.Type));

      int baseValue = 1;
      switch (coinItem.Type) {
        case ItemType.SilverCoin:
          baseValue = 100;
          break;
        case ItemType.GoldCoin:
          baseValue = 100 * 100;
          break;
        case ItemType.PlatinumCoin:
          baseValue = 100 * 100 * 100;
          break;
      }

      return baseValue * coinItem.StackSize;
    }
    #endregion
  }
}