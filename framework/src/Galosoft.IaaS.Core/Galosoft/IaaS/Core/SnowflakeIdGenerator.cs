using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading;

namespace Galosoft.IaaS.Core
{ /// <summary>
  /// Twitter的分布式自增ID雪花算法
  /// </summary>
    internal class SnowflakeIdGenerator : ISnowflakeIdGenerator
    {
        //起始的时间戳
        private static long START_STMP = 1480166465631L;

        //每一部分占用的位数
        private static int SEQUENCE_BIT = 12; //序列号占用的位数
        private static int MACHINE_BIT = 5;   //机器标识占用的位数
        private static int DATACENTER_BIT = 5;//数据中心占用的位数

        //每一部分的最大值
        private static long MAX_DATACENTER_NUM = -1L ^ -1L << DATACENTER_BIT;
        private static long MAX_MACHINE_NUM = -1L ^ -1L << MACHINE_BIT;
        private static long MAX_SEQUENCE = -1L ^ -1L << SEQUENCE_BIT;

        //每一部分向左的位移
        private static int MACHINE_LEFT = SEQUENCE_BIT;
        private static int DATACENTER_LEFT = SEQUENCE_BIT + MACHINE_BIT;
        private static int TIMESTMP_LEFT = DATACENTER_LEFT + DATACENTER_BIT;

        private long datacenterId = 1;  //数据中心
        private long machineId = 1;     //机器标识
        private long sequence = 0L; //序列号
        private long lastStmp = -1L;//上一次时间戳

        #region 单例:完全懒汉
        private static readonly Lazy<SnowflakeIdGenerator> lazy = new Lazy<SnowflakeIdGenerator>(() => new SnowflakeIdGenerator());
        public static SnowflakeIdGenerator Instance { get { return lazy.Value; } }
        private SnowflakeIdGenerator() { }
        #endregion

        public SnowflakeIdGenerator(long cid, long mid)
        {
            if (cid > MAX_DATACENTER_NUM || cid < 0) throw new Exception($"中心Id应在(0,{MAX_DATACENTER_NUM})之间");
            if (mid > MAX_MACHINE_NUM || mid < 0) throw new Exception($"机器Id应在(0,{MAX_MACHINE_NUM})之间");
            datacenterId = cid;
            machineId = mid;
        }

        /// <summary>
        /// 产生下一个ID
        /// </summary>
        /// <returns></returns>
        public long NextId()
        {
            long currStmp = getNewstmp();
            if (currStmp < lastStmp) throw new Exception("时钟倒退，Id生成失败！");

            if (currStmp == lastStmp)
            {
                //相同毫秒内，序列号自增
                sequence = sequence + 1 & MAX_SEQUENCE;
                //同一毫秒的序列数已经达到最大
                if (sequence == 0L) currStmp = getNextMill();
            }
            else
            {
                //不同毫秒内，序列号置为0
                sequence = 0L;
            }

            lastStmp = currStmp;

            return currStmp - START_STMP << TIMESTMP_LEFT       //时间戳部分
                          | datacenterId << DATACENTER_LEFT       //数据中心部分
                          | machineId << MACHINE_LEFT             //机器标识部分
                          | sequence;                             //序列号部分
        }

        private long getNextMill()
        {
            long mill = getNewstmp();
            while (mill <= lastStmp)
            {
                mill = getNewstmp();
            }
            return mill;
        }

        private long getNewstmp()
        {
            return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
        }
    }

    internal class SnowflakeId : ISnowflakeIdGenerator
    {
        /// <summary>
        /// Start time 2010-11-04 09:42:54
        /// </summary>
        public const long Twepoch = 1288834974657L;

        /// <summary>
        /// The number of bits occupied by workerId
        /// </summary>
        private const int WorkerIdBits = 10;

        /// <summary>
        /// The number of bits occupied by timestamp
        /// </summary>
        private const int TimestampBits = 41;

        /// <summary>
        /// The number of bits occupied by sequence
        /// </summary>
        private const int SequenceBits = 12;

        /// <summary>
        /// Maximum supported machine id, the result is 1023
        /// </summary>
        private const int MaxWorkerId = ~(-1 << WorkerIdBits);

        /// <summary>
        /// mask that help to extract timestamp and sequence from a long
        /// </summary>
        private const long TimestampAndSequenceMask = ~(-1L << (TimestampBits + SequenceBits));

        private readonly object _lock = new();

        /// <summary>
        /// timestamp and sequence mix in one Long
        /// highest 11 bit: not used
        /// middle  41 bit: timestamp
        /// lowest  12 bit: sequence
        /// </summary>
        private long _timestampAndSequence;

        public SnowflakeId()
        {
            if (!long.TryParse(Environment.GetEnvironmentVariable("CAP_WORKERID"), out var workerId))
                workerId = Util.GenerateWorkerId(MaxWorkerId);

            Initialize(workerId);
        }

        public SnowflakeId(long workerId)
        {
            Initialize(workerId);
        }

        /// <summary>
        /// business meaning: machine ID (0 ~ 1023)
        /// actual layout in memory:
        /// highest 1 bit: 0
        /// middle 10 bit: workerId
        /// lowest 53 bit: all 0
        /// </summary>
        private long WorkerId { get; set; }

        public virtual long NextId()
        {
            lock (_lock)
            {
                WaitIfNecessary();
                var timestampWithSequence = _timestampAndSequence & TimestampAndSequenceMask;
                return WorkerId | timestampWithSequence;
            }
        }

        /// <summary>
        /// init first timestamp and sequence immediately
        /// </summary>
        private void InitTimestampAndSequence()
        {
            var timestamp = GetNewestTimestamp();
            var timestampWithSequence = timestamp << SequenceBits;
            _timestampAndSequence = timestampWithSequence;
        }

        /// <summary>
        /// block current thread if the QPS of acquiring UUID is too high
        /// that current sequence space is exhausted
        /// </summary>
        private void WaitIfNecessary()
        {
            var currentWithSequence = ++_timestampAndSequence;
            var current = currentWithSequence >> SequenceBits;
            var newest = GetNewestTimestamp();

            if (current >= newest) Thread.Sleep(5);
        }

        /// <summary>
        /// Common method for initializing <see cref="SnowflakeId"/>
        /// </summary>
        /// <param name="workerId"></param>
        /// <exception cref="ArgumentException"></exception>
        private void Initialize(long workerId)
        {
            InitTimestampAndSequence();
            // sanity check for workerId
            if (workerId is > MaxWorkerId or < 0)
                throw new ArgumentException($"worker Id can't be greater than {MaxWorkerId} or less than 0");

            WorkerId = workerId << (TimestampBits + SequenceBits);
        }

        /// <summary>
        /// get newest timestamp relative to twepoch
        /// </summary>
        /// <returns></returns>
        private static long GetNewestTimestamp()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - Twepoch;
        }
    }

    internal static class Util
    {
        /// <summary>
        /// auto generate workerId, try using mac first, if failed, then randomly generate one
        /// </summary>
        /// <returns>workerId</returns>
        public static long GenerateWorkerId(int maxWorkerId)
        {
            try
            {
                return GenerateWorkerIdBaseOnMac();
            }
            catch
            {
                return GenerateRandomWorkerId(maxWorkerId);
            }
        }

        /// <summary>
        /// use lowest 10 bit of available MAC as workerId
        /// </summary>
        /// <returns>workerId</returns>
        private static long GenerateWorkerIdBaseOnMac()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            //exclude virtual and Loopback
            var firstUpInterface = nics.OrderByDescending(x => x.Speed).FirstOrDefault(x =>
                !x.Description.Contains("Virtual") && x.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                x.OperationalStatus == OperationalStatus.Up);
            if (firstUpInterface == null) throw new Exception("no available mac found");
            var address = firstUpInterface.GetPhysicalAddress();
            var mac = address.GetAddressBytes();

            return ((mac[4] & 0B11) << 8) | (mac[5] & 0xFF);
        }

        /// <summary>
        /// randomly generate one as workerId
        /// </summary>
        /// <returns></returns>
        private static long GenerateRandomWorkerId(int maxWorkerId)
        {
            return new Random().Next(maxWorkerId + 1);
        }
    }

    public interface ISnowflakeIdGenerator
    {
        public long NextId();
    }
}
