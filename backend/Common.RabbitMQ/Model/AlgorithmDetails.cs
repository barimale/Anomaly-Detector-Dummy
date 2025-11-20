namespace Common.RabbitMQ.Model {
    public abstract class AlgorithmDetailsBase {

        public string Id { get; set; }
        public string SessionId { get; set; }
        public bool SolutionA { get; set; }
        public bool SolutionB { get; set; }
        public bool SolutionC { get; set; }

    }

    public class AlgorithmDetailsA: AlgorithmDetailsBase {
        //intentionally left blank
    }

    public class AlgorithmDetailsB : AlgorithmDetailsBase {
        //intentionally left blank
    }

    public class AlgorithmDetailsC : AlgorithmDetailsBase {
        //intentionally left blank
    }
}
