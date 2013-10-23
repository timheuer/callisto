﻿//
// Copyright (c) 2013 Morten Nielsen
//
// Licensed under the Microsoft Public License (Ms-PL) (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://opensource.org/licenses/Ms-PL.html
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

using Callisto.Design.Common;
using Microsoft.Windows.Design;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;
using System.ComponentModel;

namespace Callisto.Design
{
	internal class MenuMetadata : AttributeTableBuilder
	{
        public MenuMetadata()
			: base()
		{
			AddCallback(typeof(Callisto.Controls.Menu),
				b =>
				{   
					b.AddCustomAttributes("Items",
						new PropertyOrderAttribute(PropertyOrder.Early),
						new CategoryAttribute(Properties.Resources.CategoryCommon),
						//The following is necessary because this is a collection of an abstract type, so we help
						//the designer with populating supported types that can be added to the collection
                        new NewItemTypesAttribute(new System.Type[] {
                            typeof(Callisto.Controls.MenuItem),
                            typeof(Callisto.Controls.MenuItemSeparator),
                            typeof(Callisto.Controls.ToggleMenuItem)
                        }),
						new AlternateContentPropertyAttribute()
					);
					b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Callisto, false));
				}
			);
		}
	}
}
