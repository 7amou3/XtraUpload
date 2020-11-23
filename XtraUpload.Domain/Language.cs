using System;

namespace XtraUpload.Domain
{
    public class Language
    {
        /// <summary>
        /// Language Id
        /// </summary>
        public Guid Id { get; set; }
        /// <summary>
        /// The displayed name of the language, ex English, Francais..
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The culture, ex en-US, fr-FR, es-ES...
        /// for more info check: https://en.wikipedia.org/wiki/Language_localisation
        /// </summary>
        public string Culture { get; set; }
    }
}
