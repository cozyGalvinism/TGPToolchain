using ReactiveUI;
using TGPToolchain.ViewModels;

namespace TGPToolchain.Helpers
{
    public class StringValue : ReactiveObject
    {
        private string _value;

        public string Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        public StringValue()
        {
            _value = "";
        }

        public StringValue(string value)
        {
            _value = value;
            Value = value;
        }
    }
}