using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Auth0.Core;
using ffxivList.Models;
using Microsoft.EntityFrameworkCore;

namespace ffxivList.Data
{
    public static class DbInitializer
    {
        public static void Initialize(FfListContext context)
        {
            context.Database.EnsureCreated();

            /* Create test user data */
            if (!context.Users.Any())
            {
                var users = new Models.User[]
                {
                    new Models.User
                    {
                        UserId = "auth0|58fcf8431504b634f54742cb",
                        UserName = "admin2",
                        UserRole = "Admin",
                        UserQuestsCompleted = 0,
                        UserLevemetesCompleted = 0,
                        UserCraftsCompleted = 0
                    },
                    new Models.User
                    {
                        UserId = "auth0|5904ebbf5d4f881ecedb18ad",
                        UserName = "user1",
                        UserRole = "User",
                        UserQuestsCompleted = 0,
                        UserLevemetesCompleted = 0,
                        UserCraftsCompleted = 0
                    }
                };

                foreach (var user in users)
                {
                    context.Users.Add(user);
                }
                context.SaveChanges();
            }

            /* Create test levemete data */
            if (!context.Levemetes.Any())
            {
                var levemetes = new Levemete[]
                {
                    new Levemete {LevemeteId = 1, LevemeteLevel = 20, LevemeteName = "Throw the Book at Him"},
                    new Levemete {LevemeteId = 2, LevemeteLevel = 20, LevemeteName = "Victory Is Mine, Not Yours"},
                    new Levemete
                    {
                        LevemeteId = 3,
                        LevemeteLevel = 25,
                        LevemeteName = "The Third Prize Is That You're Slain"
                    },
                    new Levemete {LevemeteId = 4, LevemeteLevel = 30, LevemeteName = "Bridges of Qiqirn Country"},
                    new Levemete {LevemeteId = 5, LevemeteLevel = 35, LevemeteName = "Just Making an Observation"},
                    new Levemete {LevemeteId = 6, LevemeteLevel = 40, LevemeteName = "Yellow Is the New Black"},
                    new Levemete {LevemeteId = 7, LevemeteLevel = 40, LevemeteName = "You Dropped Something"},
                    new Levemete {LevemeteId = 8, LevemeteLevel = 45, LevemeteName = "Amateur Hour"},
                    new Levemete {LevemeteId = 9, LevemeteLevel = 45, LevemeteName = "The Museum Is Closed"}
                };

                foreach (var levemete in levemetes)
                {
                    context.Levemetes.Add(levemete);
                }
                context.SaveChanges();
            }

            /* Create test quest data */
            if (!context.Quest.Any())
            {
                var quests = new Quest[]
                {
                    new Quest {QuestId = 1, QuestLevel = 30, QuestName = "Seer Folly"},
                    new Quest {QuestId = 2, QuestLevel = 35, QuestName = "Only You Can Prevent Forest Ire"},
                    new Quest {QuestId = 3, QuestLevel = 40, QuestName = "O Brother, Where Art Thou"},
                    new Quest {QuestId = 4, QuestLevel = 45, QuestName = "Yearn for the Urn"},
                    new Quest {QuestId = 5, QuestLevel = 50, QuestName = "Taint Misbehaving"},
                    new Quest {QuestId = 6, QuestLevel = 52, QuestName = "A Journey of Purification"},
                    new Quest {QuestId = 7, QuestLevel = 54, QuestName = "The Girl with the Dragon Tissue"},
                    new Quest {QuestId = 8, QuestLevel = 56, QuestName = "The Dark Blight Writhes"},
                    new Quest {QuestId = 9, QuestLevel = 58, QuestName = "Trials of the Padjals"},
                    new Quest {QuestId = 10, QuestLevel = 60, QuestName = "Hands of Healing"}
                };

                foreach (var quest in quests)
                {
                    context.Quest.Add(quest);
                }
                context.SaveChanges();
            }

            /* Create test craft data */
            if (!context.Craft.Any())
            {
                var crafts = new Craft[]
                {
                    new Craft {CraftId = 1, CraftLevel = 1, CraftName = "Distilled Water"},
                    new Craft {CraftId = 2, CraftLevel = 1, CraftName = "Quicksilver"},
                    new Craft {CraftId = 3, CraftLevel = 2, CraftName = "Animal Glue"},
                    new Craft {CraftId = 4, CraftLevel = 3, CraftName = "Growth Formula Alpha"},
                    new Craft {CraftId = 5, CraftLevel = 4, CraftName = "Enchanted Copper Ink"},
                    new Craft {CraftId = 6, CraftLevel = 4, CraftName = "Maple Wand"},
                    new Craft {CraftId = 7, CraftLevel = 5, CraftName = "Leather Grimoire"},
                    new Craft {CraftId = 8, CraftLevel = 5, CraftName = "Antidote"}
                };

                foreach (var craft in crafts)
                {
                    context.Craft.Add(craft);
                }
                context.SaveChanges();
            }

            /* Create test userlevemete data */
            if (!context.UserLevemete.Any())
            {
                foreach (var levemete in context.Levemetes)
                {
                    foreach (var user in context.Users)
                    {
                        context.UserLevemete.Add(new UserLevemete
                        {
                            IsComplete = false,
                            LevemeteId = levemete.LevemeteId,
                            UserId = user.UserId
                        });
                    }
                }
                context.SaveChanges();
            }

            /* Create test userquest data */
            if (!context.UserQuest.Any())
            {
                foreach (var quest in context.Quest)
                {
                    foreach (var user in context.Users)
                    {
                        context.UserQuest.Add(new UserQuest()
                        {
                            IsComplete = false,
                            QuestId = quest.QuestId,
                            UserId = user.UserId
                        });
                    }
                }
                context.SaveChanges();
            }

            /* Create test usercraft data */
            if (!context.UserCraft.Any())
            {
                foreach (var craft in context.Craft)
                {
                    foreach (var user in context.Users)
                    {
                        context.UserCraft.Add(new UserCraft()
                        {
                            IsComplete = false,
                            CraftId = craft.CraftId,
                            UserId = user.UserId
                        });
                    }
                }
                context.SaveChanges();
            }

            /* Create test alluserlevemete data */
            if (!context.AllUserLevemete.Any())
            {
                foreach (var userLevemete in context.UserLevemete)
                {
                    Levemete levemete = context.Levemetes.Find(userLevemete.LevemeteId);

                    context.AllUserLevemete.Add(new AllUserLevemete()
                    {
                        IsComplete = userLevemete.IsComplete,
                        LevemeteId = userLevemete.LevemeteId,
                        LevemeteLevel = levemete.LevemeteLevel,
                        LevemeteName = levemete.LevemeteName,
                        UserId = userLevemete.UserId,
                        UserLevemeteId = userLevemete.UserLevemeteId
                    });
                }
                context.SaveChanges();
            }

            /* Create test alluserquest data */
            if (!context.AllUserQuest.Any())
            {
                foreach (var userQuest in context.UserQuest)
                {
                    Quest quest = context.Quest.Find(userQuest.QuestId);

                    context.AllUserQuest.Add(new AllUserQuest()
                    {
                        IsComplete = userQuest.IsComplete,
                        QuestId = userQuest.QuestId,
                        QuestLevel = quest.QuestLevel,
                        QuestName = quest.QuestName,
                        UserId = userQuest.UserId,
                        UserQuestId = userQuest.UserQuestId
                    });
                }
                context.SaveChanges();
            }

            /* Create test allusercraft data */
            if (!context.AllUserCraft.Any())
            {
                foreach (var userCraft in context.UserCraft)
                {
                    Craft craft = context.Craft.Find(userCraft.CraftId);

                    context.AllUserCraft.Add(new AllUserCraft()
                    {
                        IsComplete = userCraft.IsComplete,
                        CraftId = userCraft.CraftId,
                        CraftLevel = craft.CraftLevel,
                        CraftName = craft.CraftName,
                        UserId = userCraft.UserId,
                        UserCraftId = userCraft.UserCraftId
                    });
                }
                context.SaveChanges();
            }
        }
    }
}
