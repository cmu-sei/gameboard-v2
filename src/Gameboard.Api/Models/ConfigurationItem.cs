// Copyright 2020 Carnegie Mellon University. All Rights Reserved.
// Released under a MIT (SEI)-style license. See LICENSE.md in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace Gameboard.Api.Models
{
    /// <summary>
    /// configuration item for api endpoints /configuration and /status
    /// for visiblity into environments for troubleshooting
    /// </summary>
    public class ConfigurationItem
    {
        List<ConfigurationItemSetting> _settings = new List<ConfigurationItemSetting>();
        public ConfigurationItem()
        {
        }

        public ConfigurationItem(string name, IEnumerable<ConfigurationItemSetting> settings)
        {
            Name = name;
            _settings.AddRange(settings);
        }

        public ConfigurationItem(string name, Dictionary<string, object> settings)
        {
            Name = name;
            _settings.AddRange(settings.Select(s => new ConfigurationItemSetting() { Key = s.Key, Value = s.Value }));
        }

        public string Name { get; set; }
        public List<ConfigurationItemSetting> Settings
        {
            get { return _settings.OrderBy(s => s.Key).ToList(); }
            set { _settings = value; }
        }
    }

    public class ConfigurationItemSetting
    {
        public string Key { get; set; }
        public object Value { get; set; }
    }
}

