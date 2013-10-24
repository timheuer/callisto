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
using System;
using System.ComponentModel;

namespace Callisto.Design
{
    internal class NumericUpDownDefaults : DefaultInitializer
    {
        public override void InitializeDefaults(ModelItem item)
        {
            item.Properties["Text"].SetValue("0");
        }
    }

	internal class NumericUpDownMetadata : AttributeTableBuilder
	{
		public NumericUpDownMetadata()
			: base()
		{
			AddCallback(typeof(Callisto.Controls.NumericUpDown),
				b =>
				{
                    b.AddCustomAttributes(new FeatureAttribute(typeof(NumericUpDownDefaults)));

					b.AddCustomAttributes("DecimalPlaces",
						new CategoryAttribute(Properties.Resources.CategoryCommon)
					);
					b.AddCustomAttributes("Delay",
						new CategoryAttribute(Properties.Resources.CategoryCommon)
					);
					b.AddCustomAttributes("Increment",
						new CategoryAttribute(Properties.Resources.CategoryCommon)
					);
					b.AddCustomAttributes("Interval",
						new CategoryAttribute(Properties.Resources.CategoryCommon)
					);
					b.AddCustomAttributes("Maximum",
						new CategoryAttribute(Properties.Resources.CategoryCommon)
					);
					b.AddCustomAttributes("Minimum",
						new CategoryAttribute(Properties.Resources.CategoryCommon)
					);
					b.AddCustomAttributes("Value",
						new CategoryAttribute(Properties.Resources.CategoryCommon)
					);
                    b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Callisto, false));
				}
			);
		}
	}
}
