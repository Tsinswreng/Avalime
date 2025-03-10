using System.Collections.Generic;
using Avalime.ViewModels;

namespace Avalime.UI;
using Ctx = TemplateVm;
public partial class TemplateVm
	:ViewModelBase
{
	public static List<Ctx> samples = [];
	static TemplateVm(){
		samples.Add(new Ctx{

		});
	}
}