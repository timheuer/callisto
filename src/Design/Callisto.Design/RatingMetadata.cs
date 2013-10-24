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
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Model;
using Microsoft.Windows.Design.PropertyEditing;
using System.ComponentModel;

namespace Callisto.Design
{
    internal class RatingDefaults : DefaultInitializer
    {
        public override void InitializeDefaults(ModelItem item)
        {
            item.Properties["ItemCount"].SetValue(5);
            item.Properties["Value"].SetValue(2.5);
        }
    }

	internal class RatingMetadata : AttributeTableBuilder
	{
		public RatingMetadata()
			: base()
		{
			AddCallback(typeof(Callisto.Controls.Rating),
				b =>
				{
                    b.AddCustomAttributes(new FeatureAttribute(typeof(RatingDefaults)));

					b.AddCustomAttributes("DisplayValue",
						new CategoryAttribute(Properties.Resources.CategoryCommon)
					);
					b.AddCustomAttributes("ItemCount",
						new CategoryAttribute(Properties.Resources.CategoryCommon)
					);
					b.AddCustomAttributes("PointerOverFill",
						new CategoryAttribute(Properties.Resources.CategoryBrush)
					);
					b.AddCustomAttributes("PointerPressedFill",
						new CategoryAttribute(Properties.Resources.CategoryBrush)
					);
					b.AddCustomAttributes("ReadOnlyFill",
						new CategoryAttribute(Properties.Resources.CategoryBrush)
					);
					b.AddCustomAttributes("SelectionMode",
						new CategoryAttribute(Properties.Resources.CategoryCommon)
					);
					b.AddCustomAttributes("Value",
						new CategoryAttribute(Properties.Resources.CategoryCommon)
					);
					b.AddCustomAttributes("WeightedValue",
						new CategoryAttribute(Properties.Resources.CategoryCommon)
					);
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Callisto, false));
				}
			);
		}
	}
}
