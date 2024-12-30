namespace HTTPServer.Decoding
{
    public class DecodedHTTPMessage
    (
     string? type_ = default,  string? targetResource_ = default, string? protocol_  =  default,
     Dictionary<string, string>?  headers_ = default, Dictionary<string, string>? body_ = default 
    )
    
    {

        #region  Properties 
        public string? Method { get => requestType; }
        public string? DesiredResource { get => targetResource; }
        public string? Protocol {get => protocol;}
        public Dictionary<string , string>? Headers {get => headers;}
        public Dictionary<string, string>? Contents { get => content; }

        #endregion

        #region  Private Fields
        private readonly string? requestType = type_;
        private readonly string? protocol =  protocol_;
        private readonly string? targetResource = targetResource_;
        
        private readonly Dictionary<string, string>? headers =  headers_ ;
        private readonly Dictionary<string, string>? content = body_;
        
        #endregion

    }

    [Flags]
    public enum HTTPVerb
    {
        GET = 1,
        POST = 2,
        DELETE = 4,
        UPDATE = 8
    }
}