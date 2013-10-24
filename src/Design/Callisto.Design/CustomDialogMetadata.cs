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
    internal class CustomDialogDefaults : DefaultInitializer
    {
        public override void InitializeDefaults(ModelItem item)
        {
            item.Content.SetValue("Custom Content Here");
            item.Properties["Title"].SetValue("Dialog Title");
        }
    }

	internal class CustomDialogMetadata : AttributeTableBuilder
	{
		public CustomDialogMetadata()
			: base()
		{
			AddCallback(typeof(Callisto.Controls.CustomDialog),
				b =>
				{
                    b.AddCustomAttributes(new FeatureAttribute(typeof(CustomDialogDefaults)));

					b.AddCustomAttributes("Title",
						new CategoryAttribute(Properties.Resources.CategoryCommon)
						);
					b.AddCustomAttributes("IsOpen",
						new CategoryAttribute(Properties.Resources.CategoryCommon)
					);
					b.AddCustomAttributes("BackButtonVisibility",
						new CategoryAttribute(Properties.Resources.CategoryAppearance)
						);
					b.AddCustomAttributes("BackButtonCommand",
						new CategoryAttribute(Properties.Resources.CategoryCommon)
					);
					b.AddCustomAttributes("BackButtonCommandParameter",
						new CategoryAttribute(Properties.Resources.CategoryCommon)
					);
					b.AddCustomAttributes(new ToolboxCategoryAttribute(ToolboxCategoryPaths.Callisto, false));
				}
			);
		}
	}
}