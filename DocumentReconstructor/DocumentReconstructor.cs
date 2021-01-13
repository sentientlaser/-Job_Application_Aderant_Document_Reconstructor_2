using System;
using System.Collections.Generic;

namespace DocumentReconstructor
{
    /// <summary>
    /// This is the behaviour of the program.
    ///
    /// Test Driven Design requires "coding to interfaces" be strictly
    /// adhered to, so this is created first and tests are written for
    /// it in advance.
    /// </summary>
    public interface IDocumentReconstructor
    {
        string Reconstruct(List<string> fragments);
    }


    /// <summary>
    /// Implementation for <see cref="IDocumentReconstructor"/>
    /// </summary>
    public class DocumentConstructorImpl : IDocumentReconstructor
    {
        public string Reconstruct(List<string> fragments)
        {
            throw new NotImplementedException();
        }
    }
}
