using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenLocoTool
{
	public class varinf
	{
		public int ofs;// offset counted from the beginning of the structure
		public int size;// size in bytes, negative for signed variables
		public int num;// number of entries (for arrays)
		public string name; // name, if known; if empty (*not* null!) will use generic field_## name
		public varinf[] structvars;// if not null, sub-structure definition
		public List<string> flags;// if not null, bit field definition (pointer to list of *char bit names)

		public varinf(int ofs, int size, int num, string name, varinf[] structvars, List<string> flags)
		{
			this.ofs = ofs;
			this.size = size;
			this.num = num;
			this.name = name;
			this.structvars = structvars;
			this.flags = flags;
		}
		public varinf(int ofs, int size, int num, string name, varinf[] structvars)
		{
			this.ofs = ofs;
			this.size = size;
			this.num = num;
			this.name = name;
			this.structvars = structvars;
		}

		public varinf(int ofs, int size, int num, string name)
		{
			this.ofs = ofs;
			this.size = size;
			this.num = num;
			this.name = name;
		}

		public varinf(int ofs)
		{
			this.ofs = ofs;
		}
	}

	public enum useobjid
	{
		ob_tracktype,
		ob_trackmod,
		ob_visualeffect,
		ob_wakeeffect,
		ob_rackrail,
		ob_compatible,
		ob_startsnd,
		ob_soundeffect,
		ob_signal,
		ob_tunnel,
		ob_bridge,
		ob_station,
		ob_cliff,
		ob_roadmod,
		ob_produces,
		ob_accepts,
		ob_fence,
		ob_cargo,
		ob_default,
		ob_LASTVALUE,
	};

	public enum desctype
	{
		objdata,
		lang,
		useobj,
		auxdata,
		auxdatafix,
		auxdatavar,
		strtable,
		cargo,
		sprites,
		sounds,
		END
	}

	public class auxdesc
	{
		string name;
		varinf[] vars;

		public auxdesc(string name, varinf[] vars)
		{
			this.name = name;
			this.vars = vars;
		}
	}

	public class objdesc
	{
		public desctype type;
		public int[] param = new int[5]; // 

		public objdesc(desctype descType, int[] param)
		{
			this.type = descType;
			this.param = param;
		}
		public objdesc(desctype descType)
		{
			this.type = descType;
		}
	}

	public class objclass
	{
		public varinf[] vars;
		public UInt32 size;
		public auxdesc[] auxdesc;
		public objdesc[] desc;

		public objclass(varinf[] vars, uint size, auxdesc[] auxdesc, objdesc[] desc)
		{
			this.vars = vars;
			this.size = size;
			this.auxdesc = auxdesc;
			this.desc = desc;
		}
	}
}
