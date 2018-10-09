using System;
using System.Runtime.InteropServices;
using System.Text;

namespace GraphQLParser.Native
{
    [StructLayout(LayoutKind.Sequential)]
    public class GraphQlDocument : IDisposable
    {
        private readonly IntPtr _pointer;
        private readonly string _error;

        public GraphQlDocument(IntPtr pointer, string error)
        {
            _pointer = pointer;
            _error = error;
        }

        public GraphQLAstLocation Location
        {
            get
            {
                GraphQlAstNodeImpl.GetLocation(_pointer, out var location);
                return location;
            }
        }

        public bool IsSuccess => _error == null;

        public string Error => _error;

        public void Dispose()
        {
            GraphQlAstNodeImpl.Free(_pointer);
            GC.SuppressFinalize(this);
        }

        ~GraphQlDocument()
        {
            GraphQlAstNodeImpl.Free(_pointer);
        }
    }
}