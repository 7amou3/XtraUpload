using System;
using System.Collections.Generic;

namespace XtraUpload.Domain
{
    public class Language
    {
        public Language()
        {
            Users = new HashSet<User>();
        }
        /// <summary>
        /// Language Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// Wether this language is the default or not
        /// </summary>
        public bool Default { get; set; }
        /// <summary>
        /// The displayed name of the language, ex English, Francais..
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The culture, ex en-US, fr-FR, es-ES...
        /// for more info check: https://en.wikipedia.org/wiki/Language_localisation
        /// </summary>
        public string Culture { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
