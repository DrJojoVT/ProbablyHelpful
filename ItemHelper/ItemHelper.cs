using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;

namespace ProbablyHelpful
{
    public partial class ProbablyHelpfulItem
    {
        private GameItem _item;
        private string _itemId;
        private string _modId;

        public ProbablyHelpfulItem(string modId, string itemId)
        {
            if (itemId.Contains(".")) {
                _itemId = itemId;
            } else {
                _itemId = modId + "." + itemId;
            }
            _modId = modId;
            _item = ItemDirectory.CreateEmptyItem(_itemId);
        }

        public ProbablyHelpfulItem SetName(string name)
        {
            _item.name = name;
            return this;
        }

        public ProbablyHelpfulItem SetDescription(string desc)
        {
            _item.shortDescription = desc;
            return this;
        }

        public ProbablyHelpfulItem SetValue(int value)
        {
            _item.SetValue(value);
            return this;
        }

        
    }
}