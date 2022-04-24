using System;
using System.Collections;
using System.Globalization;

namespace Vatsim.Vatis.Client.Common
{
    public class CommandLineParser
	{
		private interface IParsableOptionParameter
		{
			object Value { get; set; }

			object parseValue(string parameter);
		}

		public class Option : IParsableOptionParameter
		{
			private ArrayList _Names;

			private bool _matched;

			private string _name = "";

			private string _description = "";

			private object _value;

			private Type _switchType;

			private bool _needsVal;

			private bool _required;

			public string Name
			{
				get
				{
					return _name;
				}
				set
				{
					_name = value;
				}
			}

			public string Description
			{
				get
				{
					return _description;
				}
				set
				{
					_description = value;
				}
			}

			public Type Type => _switchType;

			public bool needsValue => _needsVal;

			public bool isRequired
			{
				get
				{
					return _required;
				}
				set
				{
					_required = value;
				}
			}

			public bool isMatched
			{
				get
				{
					return _matched;
				}
				set
				{
					_matched = value;
				}
			}

			public string[] Names
			{
				get
				{
					if (_Names == null)
					{
						return null;
					}
					return (string[])_Names.ToArray(typeof(string));
				}
			}

			public string[] Aliases
			{
				get
				{
					if (_Names == null)
					{
						return null;
					}
					ArrayList arrayList = new ArrayList(_Names);
					arrayList.RemoveAt(0);
					return (string[])arrayList.ToArray(typeof(string));
				}
			}

			public object Value
			{
				get
				{
					return _value;
				}
				set
				{
					_value = value;
				}
			}

			private Option()
			{
			}

			public Option(string name, string description, Type type, bool hasval, bool required)
			{
				_switchType = type;
				_needsVal = hasval;
				_required = required;
				Initialize(name, description);
			}

			private void Initialize(string name, string description)
			{
				_name = name;
				_description = description;
				_Names = new ArrayList();
				_Names.Add(name);
			}

			public void AddAlias(string alias)
			{
				if (!isASwitch(alias))
				{
					throw new CMDLineParserException("Invalid Option:'" + alias + "'::The Switch name does not start with an switch identifier '-' or '/'  or contains space!");
				}
				if (_Names == null)
				{
					_Names = new ArrayList();
				}
				_Names.Add(alias);
			}

			public void Clear()
			{
				_matched = false;
				_value = null;
			}

			public virtual object parseValue(string parameter)
			{
				if (Type == typeof(string) && needsValue)
				{
					return parameter;
				}
				throw new Exception("Option is missing an method to convert the value.");
			}
		}

		public class NumberOption : Option
		{
			public bool parseDecimalSeperator = true;

			private NumberFormatInfo _numberformat;

			private NumberStyles _numberstyle;

			public NumberFormatInfo NumberFormat
			{
				get
				{
					return _numberformat;
				}
				set
				{
					_numberformat = value;
				}
			}

			public NumberStyles NumberStyle
			{
				get
				{
					return _numberstyle;
				}
				set
				{
					_numberstyle = value;
				}
			}

			public NumberOption(string name, string description, Type type, bool hasval, bool required)
				: base(name, description, type, hasval, required)
			{
				_numberformat = new CultureInfo("", useUserOverride: false).NumberFormat;
			}

			public override object parseValue(string parameter)
			{
				if (base.Type == typeof(int))
				{
					return parseIntValue(parameter);
				}
				if (base.Type == typeof(double))
				{
					return parseDoubleValue(parameter);
				}
				throw new ParameterConversionException("Invalid Option Type: " + base.Type);
			}

			private int parseIntValue(string parameter)
			{
				try
				{
					return int.Parse(parameter, _numberstyle, _numberformat);
				}
				catch (Exception ex)
				{
					throw new ParameterConversionException("Invalid Int Parameter:" + parameter + " - " + ex.Message);
				}
			}

			private double parseDoubleValue(string parameter)
			{
				if (parseDecimalSeperator)
				{
					SetIdentifiedDecimalSeperator(parameter);
				}
				try
				{
					return double.Parse(parameter, _numberstyle, _numberformat);
				}
				catch (Exception ex)
				{
					throw new ParameterConversionException("Invalid Double Parameter:" + parameter + " - " + ex.Message);
				}
			}

			private void SetIdentifiedDecimalSeperator(string parameter)
			{
				if (_numberformat.NumberDecimalSeparator == "." && parameter.Contains(",") && !parameter.Contains("."))
				{
					_numberformat.NumberDecimalSeparator = ",";
					if (_numberformat.NumberGroupSeparator == ",")
					{
						_numberformat.NumberGroupSeparator = ".";
					}
				}
				else if (_numberformat.NumberDecimalSeparator == "," && parameter.Contains(".") && !parameter.Contains(","))
				{
					_numberformat.NumberDecimalSeparator = ".";
					if (_numberformat.NumberGroupSeparator == ".")
					{
						_numberformat.NumberGroupSeparator = ",";
					}
				}
			}
		}

		public class CMDLineParserException : Exception
		{
			public CMDLineParserException(string message)
				: base(message)
			{
			}
		}

		public class MissingRequiredOptionException : CMDLineParserException
		{
			public MissingRequiredOptionException(string message)
				: base(message)
			{
			}
		}

		public class InvalidOptionsException : CMDLineParserException
		{
			public InvalidOptionsException(string message)
				: base(message)
			{
			}
		}

		public class DuplicateOptionException : CMDLineParserException
		{
			public DuplicateOptionException(string message)
				: base(message)
			{
			}
		}

		public class ParameterConversionException : CMDLineParserException
		{
			public ParameterConversionException(string message)
				: base(message)
			{
			}
		}

		private string[] _cmdlineArgs;

		private ArrayList SwitchesStore;

		private ArrayList _matchedSwitches;

		private ArrayList _unmatchedArgs;

		private ArrayList _invalidArgs;

		private Option _help;

		public bool collectInvalidOptions = true;

		public bool throwInvalidOptionsException;

		public Option AddHelpOption()
		{
			_help = AddBoolSwitch("-help", "Command line help");
			_help.AddAlias("-h");
			_help.AddAlias("-?");
			_help.AddAlias("/help");
			return _help;
		}

		public bool Parse(string[] args)
		{
			Clear();
			_cmdlineArgs = args;
			ParseOptions();
			if (_invalidArgs.Count > 0)
			{
				if (throwInvalidOptionsException)
				{
					string text = "";
					foreach (string invalidArg in _invalidArgs)
					{
						text = text + "'" + invalidArg + "';";
					}
					throw new InvalidOptionsException("Invalid command line argument(s): " + text);
				}
				return false;
			}
			return true;
		}

		public void Clear()
		{
			_matchedSwitches = null;
			_unmatchedArgs = null;
			_invalidArgs = null;
			if (SwitchesStore == null)
			{
				return;
			}
			foreach (Option item in SwitchesStore)
			{
				item.Clear();
			}
		}

		public void AddOption(Option opt)
		{
			CheckCmdLineOption(opt.Name);
			if (SwitchesStore == null)
			{
				SwitchesStore = new ArrayList();
			}
			SwitchesStore.Add(opt);
		}

		public Option AddBoolSwitch(string name, string description)
		{
			Option option = new Option(name, description, typeof(bool), hasval: false, required: false);
			AddOption(option);
			return option;
		}

		public Option AddStringParameter(string name, string description, bool required)
		{
			Option option = new Option(name, description, typeof(string), hasval: true, required);
			AddOption(option);
			return option;
		}

		public NumberOption AddIntParameter(string name, string description, bool required)
		{
			NumberOption numberOption = new NumberOption(name, description, typeof(int), hasval: true, required);
			numberOption.NumberStyle = NumberStyles.Integer;
			AddOption(numberOption);
			return numberOption;
		}

		public NumberOption AddDoubleParameter(string name, string description, bool required)
		{
			NumberOption numberOption = new NumberOption(name, description, typeof(double), hasval: true, required);
			numberOption.NumberStyle = NumberStyles.Float;
			AddOption(numberOption);
			return numberOption;
		}

		public NumberOption AddDoubleParameter(string name, string description, bool required, NumberFormatInfo numberformat)
		{
			NumberOption numberOption = new NumberOption(name, description, typeof(double), hasval: true, required);
			numberOption.NumberFormat = numberformat;
			numberOption.parseDecimalSeperator = false;
			numberOption.NumberStyle = NumberStyles.Float | NumberStyles.AllowThousands;
			AddOption(numberOption);
			return numberOption;
		}

		private void CheckCmdLineOption(string name)
		{
			if (!isASwitch(name))
			{
				throw new CMDLineParserException("Invalid Option:'" + name + "'::The Switch name does not start with an switch identifier '-' or '/'  or contains space!");
			}
		}

		protected static bool isASwitch(string arg)
		{
			return (arg.StartsWith("-") || arg.StartsWith("/")) & !arg.Contains(" ");
		}

		private void ParseOptions()
		{
			_matchedSwitches = new ArrayList();
			_unmatchedArgs = new ArrayList();
			_invalidArgs = new ArrayList();
			if (_cmdlineArgs == null || SwitchesStore == null)
			{
				return;
			}
			for (int i = 0; i < _cmdlineArgs.Length; i++)
			{
				string arg = _cmdlineArgs[i];
				bool flag = false;
				foreach (Option item in SwitchesStore)
				{
					if (compare(item, arg))
					{
						flag = (item.isMatched = true);
						_matchedSwitches.Add(item);
						i = processMatchedSwitch(item, _cmdlineArgs, i);
					}
				}
				if (!flag)
				{
					processUnmatchedArg(arg);
				}
			}
			checkReqired();
		}

		private void checkReqired()
		{
			foreach (Option item in SwitchesStore)
			{
				if (item.isRequired && !item.isMatched)
				{
					throw new MissingRequiredOptionException("Missing Required Option:'" + item.Name + "'");
				}
			}
		}

		private bool compare(Option s, string arg)
		{
			string[] names;
			if (!s.needsValue)
			{
				names = s.Names;
				foreach (string text in names)
				{
					if (text.Equals(arg))
					{
						s.Name = text;
						return true;
					}
				}
				return false;
			}
			names = s.Names;
			foreach (string text2 in names)
			{
				if (arg.StartsWith(text2))
				{
					checkDuplicateAndSetName(s, text2);
					return true;
				}
			}
			return false;
		}

		private void checkDuplicateAndSetName(Option s, string optname)
		{
			if (s.isMatched && s.needsValue)
			{
				throw new DuplicateOptionException("Duplicate: The Option:'" + optname + "' allready exists on the comand line as  +'" + s.Name + "'");
			}
			s.Name = optname;
		}

		private int retrieveParameter(ref string parameter, string optname, string[] cmdlineArgs, int pos)
		{
			if (cmdlineArgs[pos].Length == optname.Length)
			{
				if (cmdlineArgs.Length > pos + 1)
				{
					pos++;
					parameter = cmdlineArgs[pos];
				}
			}
			else
			{
				parameter = cmdlineArgs[pos].Substring(optname.Length);
			}
			return pos;
		}

		protected int processMatchedSwitch(Option s, string[] cmdlineArgs, int pos)
		{
			if (s.Type == typeof(bool) && !s.needsValue)
			{
				s.Value = true;
				return pos;
			}
			if (s.needsValue)
			{
				string parameter = "";
				pos = retrieveParameter(ref parameter, s.Name, cmdlineArgs, pos);
				try
				{
					if (s.Type != null)
					{
						((IParsableOptionParameter)s).Value = ((IParsableOptionParameter)s).parseValue(parameter);
						return pos;
					}
				}
				catch (Exception ex)
				{
					throw new ParameterConversionException(ex.Message);
				}
			}
			throw new CMDLineParserException("Unsupported Parameter Type:" + s.Type);
		}

		protected void processUnmatchedArg(string arg)
		{
			if (collectInvalidOptions && isASwitch(arg))
			{
				_invalidArgs.Add(arg);
			}
			else
			{
				_unmatchedArgs.Add(arg);
			}
		}

		public string[] RemainingArgs()
		{
			if (_unmatchedArgs == null)
			{
				return null;
			}
			return (string[])_unmatchedArgs.ToArray(typeof(string));
		}

		public string[] matchedOptions()
		{
			if (_matchedSwitches == null)
			{
				return null;
			}
			ArrayList arrayList = new ArrayList();
			for (int i = 0; i < _matchedSwitches.Count; i++)
			{
				arrayList.Add(((Option)_matchedSwitches[i]).Name);
			}
			return (string[])arrayList.ToArray(typeof(string));
		}

		public string[] invalidArgs()
		{
			if (_invalidArgs == null)
			{
				return null;
			}
			return (string[])_invalidArgs.ToArray(typeof(string));
		}

		public string HelpMessage()
		{
			int length = "  ".Length;
			int num = 0;
			foreach (Option item in SwitchesStore)
			{
				string[] names = item.Names;
				for (int i = 0; i < names.Length; i++)
				{
					int num2 = names[i].Length;
					if (item.needsValue)
					{
						num2 += " [..]".Length;
					}
					num = Math.Max(num, num2);
				}
			}
			string text = "\nCommand line options are:\n\n";
			bool flag = false;
			foreach (Option item2 in SwitchesStore)
			{
				string text2 = "  " + item2.Names[0];
				if (item2.needsValue)
				{
					text2 += " [..]";
				}
				while (text2.Length < num + 3 + length)
				{
					text2 += " ";
				}
				if (item2.isRequired)
				{
					text2 += "(*) ";
					flag = true;
				}
				text2 += item2.Description;
				text = text + text2 + "\n";
				if (item2.Aliases != null && item2.Aliases.Length != 0)
				{
					string[] names = item2.Aliases;
					foreach (string text3 in names)
					{
						text2 = "  " + text3;
						if (item2.needsValue)
						{
							text2 += " [..]";
						}
						text = text + text2 + "\n";
					}
				}
				text += "\n";
			}
			if (flag)
			{
				text += "(*) Required.\n";
			}
			return text;
		}
	}
}