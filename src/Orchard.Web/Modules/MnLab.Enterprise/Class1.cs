using System.Collections.Generic;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Records;
using System.Linq;

namespace RelationSample.Models {

    public class RewardsPart : ContentPart<RewardsPartRecord> {
        public IEnumerable<RewardProgramRecord> Rewards {
            get {
                return Record.Rewards.Select(r => r.RewardProgramRecord);
            }
        }
    }

    public class RewardsPartRecord : ContentPartRecord {
        public RewardsPartRecord() {
            Rewards = new List<ContentRewardProgramsRecord>();
        }
        public virtual IList<ContentRewardProgramsRecord> Rewards { get; set; }
    }


    public class ContentRewardProgramsRecord {
        public virtual int Id { get; set; }
        public virtual RewardsPartRecord RewardsPartRecord { get; set; }
        public virtual RewardProgramRecord RewardProgramRecord { get; set; }
    }

    public class RewardProgramRecord {
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual double Discount { get; set; }
    }


}

