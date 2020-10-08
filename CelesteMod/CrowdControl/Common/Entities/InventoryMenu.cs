using System;
using System.Collections.Generic;
using System.ComponentModel;
using CrowdControl.Common.Interfaces;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace CrowdControl.Common
{
    [Serializable]
    public class InventoryMenu : IEquatable<InventoryMenu>//, IMenu
    {
        [CanBeNull, JsonProperty(PropertyName = "id")]
        public uint? ID;
        //[JsonIgnore] uint? IMenu.ID => ID;

        [NotNull, JsonProperty(PropertyName = "name")]
        public string Name = string.Empty;
        //[JsonIgnore] string IMenu.Name => Name;

        [CanBeNull, JsonProperty(PropertyName = "channel")]
        public Channel Channel;
        //[JsonIgnore] Channel IMenu.Channel => Channel;

        [NotNull, JsonProperty(PropertyName = "game")]
        public IGame Game;
        //[JsonIgnore] IGame IMenu.Game => Game;

        [JsonProperty(PropertyName = "remote")]
        public bool Remote;
        //[JsonIgnore] bool IMenu.Remote => Remote;

        [NotNull, ItemNotNull, JsonProperty(PropertyName = "items")]
        public List<IItem> Items = new List<IItem>();
        //[JsonIgnore] IEnumerable<IItem> IMenu.Items => Items.Cast<IItem>();

        public enum TemplateType : byte
        {
            [Description("Not a Template")]
            NotTemplate = 0,
            [Description("Partner Template")]
            Partner = 1,
            [Description("Affiliate Template")]
            Affiliate = 2,
            [Description("Community Template")]
            Community = 4
        }

        public InventoryMenu() { }

        public InventoryMenu([NotNull] InventoryMenu menu, [NotNull] Channel channel)
        {
            ID = menu.ID;
            Name = menu.Name;
            Channel = channel;
            Game = menu.Game;
            Items.AddRange(menu.Items);
        }

        public InventoryMenu([NotNull] string name, [NotNull] Channel channel, [NotNull] Game game)
        {
            Name = name;
            Channel = channel;
            Game = game;
        }

        public bool Equals(InventoryMenu other) => ID.Equals(other?.ID);

#if NET471 || NETCOREAPP
        public (bool success, string message) Validate(Channel channel) => throw new NotImplementedException();
#endif
    }
}
