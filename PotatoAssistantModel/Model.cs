using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;
using System.IO;
namespace PotatoAssistantModel
{
    /// <summary>
    /// A Target represents the desired share that a fund should hold out from the total assets.
    /// For example: A user might want the International Potato Fund to hold 30% of his investments.
    ///
    /// This interface should be used by any class to get information about targets in a plan.
    /// To modify targets, client classes should use the Plan class.
    /// 
    /// </summary>
    public interface ITarget
    {
        string ID { get; }
        string FundName { get; }
        double Share { get;  }
        double FundMinimumPurchase { get; }
        
    }

    /// <summary>
    /// This class represents a set of Targets
    /// </summary>
    public class Plan : IEnumerable<ITarget>
    {




        /// <summary>
        /// Implementation of an ITarget that performs some checks.
        /// </summary>
        internal class Target : ITarget
        {
            private static int LAST=0;
            private string _fundName = "Potato Fund";
            private double share;
            private double _fundMinimumPurchase;
            // used to order funds
            public int creationTime;
            private string _id;

            /// <summary>
            /// The name of the fund. i.e. "TD Canadian Bond Index -e"
            /// </summary>
            public string FundName
            {
                get { return _fundName; }
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("The name of the fund can not be empty");
                    }
                    else if (value.Contains(Portfolio.SEPARATOR_CHAR.ToString())){
                        throw new ArgumentException("The name must not contain the characte '|'");
                    }
                    else if (!char.IsLetterOrDigit(value, 0))
                    {
                        throw new ArgumentException("The name of the fund must start with a letter or digit");
                    }
                    else if (char.IsWhiteSpace(value, value.Length-1))
                    {
                        throw new ArgumentException("The name of the fund must not end with spaces");
                    }

                    else
                    {
                        _fundName = value;
                    }
                }
            }

            /// <summary>
            /// An unique ID for internal purposes
            /// </summary>
            public string ID
            {
                get { return _id; }
                set
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        throw new ArgumentException("The id can not be empty");
                    }
                    else if (value.Contains(Portfolio.SEPARATOR_CHAR.ToString()))
                    {
                        throw new ArgumentException("The id must not contain the characte '|'");
                    }
                    else
                    {
                        _id = value;
                    }
                }
            }


            /// <summary>
            /// The targeted share of the fund, as a percentage (0-100)
            /// </summary>
            public double Share
            {
                get { return share; }
                set
                {
                    if (value < 0 || value > 100)
                    {
                        throw new ArgumentOutOfRangeException("The target share must be between 0 and 100.");
                    }
                    else
                    {
                        share = value;
                    }
                }
            }

            /// <summary>
            /// The minimal purchase for this found (i.e. 100 dollars)
            /// </summary>
            public double FundMinimumPurchase
            {
                get { return _fundMinimumPurchase; }

                set
                {
                    if (value < 0) { throw new ArgumentException("Minimum purchase can not be a negative number!"); }
                    else { _fundMinimumPurchase = value; }
                }
            }


            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="name">The name of the fund</param>
            /// <param name="minimalPurchase">The minimal purchase allowed for this fund</param>
            /// <param name="share">The targeted share</param>
            public Target(string name, double minimumPurchase, double share, string id)
            {
                FundName = name;
                FundMinimumPurchase = minimumPurchase;
                Share = share;
                ID = id;
                creationTime = LAST;
                LAST++;
            }


        }
        
        private Dictionary<string, Target> _targets;
        
        #region constructor
        /// <summary>
        /// Constructs an empty plan
        /// </summary>
        public Plan()
        {
            _targets = new Dictionary<string, Target>();
        }
        #endregion

        #region model-logic

        /// <summary>
        /// Add a new Target with a target
        /// </summary>
        /// <param name="fundName">The name of the fund</param>
        /// <param name="minimumPurchase">the minimal purchase for that fund</param>
        /// <returns>The created target</returns>
        public ITarget AddTarget(string fundName, double minimumPurchase, string id)
        {
            if (ContainsFundName(fundName))
            {
                throw new ArgumentException("Duplicated fund name!");
            } 
            else if(fundName.Contains(Portfolio.SEPARATOR_CHAR)){
                throw new ArgumentException("Fund's name can not contain the character '"+Portfolio.SEPARATOR_CHAR+"'");
            }
            else {
                Target fund = new Target(fundName, minimumPurchase, 0, id);
                _targets.Add(id, fund);
                return fund;
            }
        }

        /// <summary>
        /// remove a target
        /// </summary>
        /// <param name="id">the id of the target to remove</param>
        /// <returns>whether or not the target was removed</returns>
        public bool RemoveTarget(string id)
        {
            return _targets.Remove(id);
        }

        /// <summary>
        /// Overloading [] to get a Target by id
        /// </summary>
        /// <param name="id">The id of the target's fund</param>
        /// <returns>The target for the given fund</returns>
        public ITarget this[string id]
        {
            get { return _targets[id]; }
        }

        /// <summary>
        /// Change the name of a given fund, checking if it is unique
        /// </summary>
        /// <param name="oldName">The current name</param>
        /// <param name="newName">The desired new name</param>
        public void SetTargetFundName(string id, string newName)
        {
            if (_targets[id].FundName.Equals(newName))
            {
                return;
            }
            else if (ContainsFundName(newName))
            {
                throw new ArgumentException("Duplicate fund names");
            }
            _targets[id].FundName = newName;

        }


        public double ShareLeft()
        {
            return 100 - _targets.Values.Sum(target => target.Share);
        }

        public void SetMinimumPurchase(string id, double value)
        {
            _targets[id].FundMinimumPurchase = value;
        }

        /// <summary>
        /// set the values of the shares
        /// </summary>
        /// <param name="newValues"></param>
        public void SetTargetShares(Dictionary<string, double> newValues)
        {
            // Create a temporal dictionary holding the current share values
            Dictionary<string, double> temporalShares = new Dictionary<string, double>();
            foreach (KeyValuePair<string, Target> kv in _targets)
            {
                temporalShares[kv.Key] = kv.Value.Share;
            }
            
            // update the share values with the new ones in the temporal dictionary
            foreach (KeyValuePair<string, double> kv in newValues)
            {
                if ((kv.Value < 0) || (kv.Value > 100))
                {
                    throw new ArgumentException("Shares should have a value between 0 and 100.");
                }
                else if (!ContainsFund(kv.Key))
                {
                    throw new ArgumentException(kv.Key + " fund does not exist in the current plan.");
                }
                else
                {
                    temporalShares[kv.Key] = kv.Value;
                }
            }
            
            //check the new shares sum is equal to 100
            if(temporalShares.Values.Sum() > 100){
                throw new ArgumentException("Wrong modification, new shares should not sum more than 100%");    
            }

            //If it passed all the tests, set the new values, finally!
            foreach (KeyValuePair<string, double> kv in temporalShares)
            {
                _targets[kv.Key].Share = kv.Value;
            }
        }

        /// <summary>
        /// Check if a fund exists in this plan
        /// </summary>
        /// <param name="fundId">The id of the fund</param>
        /// <returns>Wheter the fund exists or not</returns>
        public bool ContainsFund(string fundId)
        {
            return _targets.ContainsKey(fundId);
        }

        /// <summary>
        /// Check if a fund exists in this plan
        /// </summary>
        /// <param name="fundname">The name of the fund</param>
        /// <returns>Wheter the fund exists or not</returns>
        public bool ContainsFundName(string fundname)
        {
            return _targets.Values.Any(target => target.FundName.Equals(fundname));
        }



        #endregion

        #region enumerating-funds
        public IEnumerator<ITarget> GetEnumerator()
        {
            return _targets.Values.OrderBy(target => target.creationTime).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion

        #region marshalling
        

        private static string[] MarshallTarget(ITarget target)
        {
            return new string[] {
                    target.ID,
                    target.FundName, 
                    Convert.ToString(target.Share, CultureInfo.InvariantCulture), 
                    Convert.ToString(target.FundMinimumPurchase, CultureInfo.InvariantCulture) 
                };
        }

        public void Marshall(StreamWriter writer)
        {
            foreach (Target target in _targets.Values)
            {
                writer.WriteLine(string.Join(Portfolio.SEPARATOR_CHAR.ToString(), MarshallTarget(target)));
            }
            writer.WriteLine("#####");
        }

        public static Plan Unmarshall(StreamReader reader)
        {
            Plan plan = new Plan();
            string line;
            Dictionary<string, double> targets = new Dictionary<string, double>();
            while ((line = reader.ReadLine()) != null)
            {
                if (line.Trim().Equals("#####"))
                {
                    break;
                }
                else if (!string.IsNullOrWhiteSpace(line))
                {
                    string[] targetArgs = line.Split(new char[] { Portfolio.SEPARATOR_CHAR });
                    string id = targetArgs[0];
                    string fundName = targetArgs[1];
                    plan.AddTarget(fundName, double.Parse(targetArgs[3], CultureInfo.InvariantCulture), id);
                    targets[id] = double.Parse(targetArgs[2], CultureInfo.InvariantCulture);
                }
            }
            plan.SetTargetShares(targets);
            return plan;
        }

        #endregion

    }

    #region history
    /// <summary>
    /// the abstract class that represents an action in the portfolio
    /// </summary>
    public abstract class Entry {

        public Dictionary<string, double> Values;

        /// <summary>
        /// Time of the action
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Return an array of string that represent the entry
        /// </summary>
        /// <returns></returns>
        public string[] Marshall()
        {
            string[] pairs = new string[Values.Keys.Count * 2];
            int offset = 0;
            foreach (KeyValuePair<string, double> pair in Values)
            {
                pairs[offset * 2] = pair.Key;
                pairs[(offset * 2) + 1] = Convert.ToString(pair.Value, CultureInfo.InvariantCulture);
                offset++;
            }
            return pairs;
        }
        

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="time">When</param>
        public Entry(DateTime time, Dictionary<string, double> newValues)
        {
            Time = time;
            Values = newValues;
        }

        /// <summary>
        /// return the names of the funds involved in this entry
        /// </summary>
        /// <returns></returns>
        public IEnumerator<string> InvolvedFunds()
        {
            return (IEnumerator<string>)Values.Keys.GetEnumerator();
        }

        /// <summary>
        /// helper for unmarshalling
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Dictionary<string, double> ExtractValues(string[] args)
        {
            Dictionary<string, double> newValues = new Dictionary<string, double>();
            for (int i = 0; i < args.Length / 2; i++)
            {
                newValues[args[(i * 2)]] = double.Parse(args[(i * 2) + 1], CultureInfo.InvariantCulture);
            }
            return newValues;
        }

        


    }

    /// <summary>
    /// this entry represents an update in the values of the funds not caused by a direct
    /// action of the user.
    /// </summary>
    public class Update : Entry
    {
        

        public Update(DateTime time, Dictionary<string, double> newValues) : base(time, newValues)
        {
        }


        public static Update Unmarshall(DateTime time, string[] args)
        {
            Dictionary<string, double> newValues = ExtractValues(args);
            return new Update(time, newValues);
        }

        
    }

    /// <summary>
    /// this class represents and actions of purchasing or selling 
    /// </summary>
    public class Transaction : Entry
    {
        
        public Transaction(DateTime time, Dictionary<string, double> values)
            : base(time, values)
        {
        }

        public static Transaction Unmarshall(DateTime time, string[] args)
        {
            Dictionary<string, double> values = ExtractValues(args);
            return new Transaction(time, values);
        }
    }

    public class History : IEnumerable<Entry>
    {

        #region entries-marshalling-infrastructure
        public delegate Entry ReadEntry(DateTime time, string [] args);
        private static Dictionary<Type, string> TypeNames = new Dictionary<Type,string>(){{typeof(Update), "UPDATE"}, {typeof(Transaction), "TRANSACTION"}};
        private static Dictionary<string, ReadEntry> Readers = new Dictionary<string, ReadEntry>(){
            {"UPDATE", (DateTime time,string[] args) => {return Update.Unmarshall(time, args);}},
            {"TRANSACTION", (DateTime time,string[] args) => {return Transaction.Unmarshall(time, args);}}
        };
        #endregion

        private List<Entry> _entries;
        private Entry _lastEntry;
        public Entry LastEntry { get { return _lastEntry; } 
                                 private set { _lastEntry = value; } }

        public void AddEntry(Entry entry)
        {
            _entries.Add(entry);
            LastEntry = entry;
        }

        public History()
        {
            _entries = new List<Entry>();
        }

        public IEnumerator<Entry> GetEnumerator()
        {
            return _entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Marshall(StreamWriter writer)
        {
            
            foreach (Entry entry in _entries)
            {
                writer.WriteLine(TypeNames[entry.GetType()]+Portfolio.SEPARATOR_CHAR+Convert.ToString(entry.Time, CultureInfo.InvariantCulture)+Portfolio.SEPARATOR_CHAR+string.Join(Portfolio.SEPARATOR_CHAR.ToString(), entry.Marshall()));
            }
        }

        public static History Unmarshall(StreamReader reader)
        {
            History history = new History();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    string[] args = line.Split(new char[]{Portfolio.SEPARATOR_CHAR});
                    history.AddEntry(Readers[args[0]](Convert.ToDateTime(args[1], CultureInfo.InvariantCulture), args.Skip(2).ToArray()));

                }
            }
            return history;
        }

    }
    #endregion

    /// <summary>
    /// A Portfolio is a plan plus a history
    /// </summary>
    public class Portfolio
    {
        public static char SEPARATOR_CHAR = '|';

        public Plan Plan { get; set; }
        public History History { get; set; }

        public Portfolio()
        {
            Plan = new Plan();
            History = new History();
        }

        /// <summary>
        /// Read a portfolio from a stream
        /// </summary>
        /// <param name="reader">the input stream</param>
        /// <returns>the portfolio</returns>
        public static Portfolio Unmarshall(StreamReader reader)
        {
            Portfolio portfolio = new Portfolio();
            portfolio.Plan = Plan.Unmarshall(reader);
            portfolio.History = History.Unmarshall(reader);
            return portfolio;
        }

        /// <returns>wheter all targets have a value or not</returns>
        public bool AllFundsHaveValue()
        {
            return History.LastEntry != null && Plan.All(target => History.LastEntry.Values.ContainsKey(target.ID));
        }

        /// <summary>
        /// Return the balance of the current funds in the Plan
        /// </summary>
        /// <returns></returns>
        public Dictionary<ITarget, double> GetBalance(bool initializeToZero)
        {
            Dictionary<ITarget, double> shares = new Dictionary<ITarget, double>();
            foreach (ITarget target in Plan)
            {
                shares[target] = (History.LastEntry != null) && (History.LastEntry.Values.ContainsKey(target.ID)) ? History.LastEntry.Values[target.ID] : (initializeToZero? 0 : double.NaN);
            }
            return shares;
        }

        /// <summary>
        /// Serialize the portfolio to a stream
        /// </summary>
        /// <param name="writer">the output stream</param>
        public void Marshall(StreamWriter writer)
        {
            Plan.Marshall(writer);
            History.Marshall(writer);
        }
    }
}
