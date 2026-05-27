using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using vtt_campaign_wiki.Server.Features.Campaign;
using vtt_campaign_wiki.Server.Features.Player;
using vtt_campaign_wiki.Server.Features.Shared.Constants;

namespace vtt_campaign_wiki.Server.Data
{
    public static class MalachiCampaignSeeder
    {
        private const string CampaignTitle = "The Crownless Halo";
        private const string SeedUserName = "admin";

        public static async Task SeedAsync( IServiceProvider serviceProvider )
        {
            var context = serviceProvider.GetRequiredService<VttCampaignWikiDbContext>();

            if (await context.Campaigns.AnyAsync( c => c.Title == CampaignTitle ))
            {
                return;
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<PlayerEntity>>();
            var owner = await userManager.FindByNameAsync( SeedUserName );
            if (owner == null)
            {
                return;
            }

            var campaign = BuildCampaign( owner.Id );

            await context.Campaigns.AddAsync( campaign );
            await context.SaveChangesAsync();
        }

        private static CampaignEntity BuildCampaign( int authorId )
        {
            var campaign = new CampaignEntity
            {
                Title = CampaignTitle,
                Content = CampaignOverviewContent,
                Position = ItemBase.POSITION_GAP,
                AuthorId = authorId,
                Items = new List<CampaignItemEntity>(),
                Players = new List<CampaignPlayerEntity>
                {
                    new CampaignPlayerEntity { PlayerId = authorId, IsDM = true },
                },
            };

            var npcs       = AddSection( campaign, "NPCs and Factions", 1, authorId );
            /* locations */  AddSection( campaign, "Locations",         2, authorId );
            var items      = AddSection( campaign, "Items",             3, authorId );
            var lore       = AddSection( campaign, "Lore",              4, authorId );
            /* houseRules*/  AddSection( campaign, "House Rules",       5, authorId );

            var partyFolder = AddChild( campaign, npcs, "Player Characters", 1, authorId );
            AddChild( campaign, partyFolder, "Malachi Dawnward", 1, authorId, MalachiContent );
            AddChild( campaign, partyFolder, "Rafe Blackthorne", 2, authorId, RafeContent );
            AddChild( campaign, partyFolder, "Edrin Vale",       3, authorId, EdrinContent );
            AddChild( campaign, partyFolder, "Roderic Ash",      4, authorId, RodericContent );
            AddChild( campaign, partyFolder, "Drenic Crow",      5, authorId, DrenicContent );

            AddChild( campaign, items, "The Crownless Halo", 1, authorId, CrownlessHaloContent );

            var faithsFolder = AddChild( campaign, lore, "Faiths", 1, authorId );
            AddChild( campaign, faithsFolder, "Lathander, the Morninglord", 1, authorId, LathanderContent );
            AddChild( campaign, faithsFolder, "The Dawn Cataclysm",         2, authorId, DawnCataclysmContent );
            AddChild( campaign, faithsFolder, "Lathander vs Amaunator",     3, authorId, LathanderVsAmaunatorContent );
            AddChild( campaign, faithsFolder, "Opposing Faiths",            4, authorId, OpposingFaithsContent );

            AddChild( campaign, lore, "Party Symbolism", 2, authorId, PartySymbolismContent );

            return campaign;
        }

        private static CampaignItemEntity AddSection( CampaignEntity campaign, string title, int positionIndex, int authorId )
        {
            var item = new CampaignItemEntity
            {
                Title = title,
                AuthorId = authorId,
                Position = positionIndex * ItemBase.POSITION_GAP,
                Campaign = campaign,
                Children = new List<CampaignItemEntity>(),
            };
            campaign.Items.Add( item );
            return item;
        }

        private static CampaignItemEntity AddChild( CampaignEntity campaign, CampaignItemEntity parent, string title, int positionIndex, int authorId, string content = "" )
        {
            var item = new CampaignItemEntity
            {
                Title = title,
                Content = content,
                AuthorId = authorId,
                Position = positionIndex * ItemBase.POSITION_GAP,
                Campaign = campaign,
                ParentEntity = parent,
                Children = new List<CampaignItemEntity>(),
            };
            parent.Children ??= new List<CampaignItemEntity>();
            parent.Children.Add( item );
            return item;
        }

        private const string CampaignOverviewContent = """
            <h2>The Crownless Halo</h2>
            <p>A campaign centered on a relic of inspiration and loyalty &mdash; and the question of whether power that bends the heart can ever be holy.</p>
            <p>The party is bound to this mystery: <strong>thorn, vale, ash, and dawn</strong>.</p>
            <p><em>&ldquo;A crownless halo is still a crown, if men kneel before it.&rdquo;</em></p>
            """;

        private const string MalachiContent = """
            <h2>Malachi Dawnward</h2>
            <p><strong>The Light of Lathander</strong></p>

            <h3>Core Identity</h3>
            <ul>
                <li><strong>Race:</strong> Aasimar</li>
                <li><strong>Class:</strong> Cleric &mdash; Light Domain</li>
                <li><strong>Faith:</strong> Lathander, the Morninglord</li>
                <li><strong>Alignment:</strong> Lawful Good</li>
                <li><strong>Background:</strong> Acolyte</li>
                <li><strong>Visual:</strong> Scale mail, shield, mace, sacred vestments, radiant celestial presence.</li>
            </ul>
            <p>Malachi does not merely worship Lathander. He believes he has been <em>kindled</em> by Lathander for a purpose: a bearer of light, a revealer of corruption, a protector of new beginnings, and a judge of things that hide in shadow.</p>
            <p>His faith is sincere, but not soft. He is charitable, disciplined, traditional, and protective. He also judges others harshly, sees omens everywhere, and can become dangerously inflexible once he believes Lathander has shown him the path.</p>

            <h3>Roleplay Thesis</h3>
            <p><em>&ldquo;There are no failed dawns &mdash; only dawns betrayed.&rdquo;</em></p>
            <p>He is loyal to Lathander to the point that he does not believe the Morninglord truly errs. If something holy fails, Malachi assumes corruption, sabotage, weakness, or shadowed interference. He is not a hypocrite. He is not pretending. He is a man whose faith is bright enough to inspire others and rigid enough to blind him.</p>

            <h3>Personality Pillars</h3>
            <ol>
                <li><strong>The Light Reveals.</strong> Light is not only comforting; it exposes lies, corruption, cowardice, and sin. <em>&ldquo;The light heals the faithful and reveals the wicked.&rdquo;</em></li>
                <li><strong>Dawn Means Renewal.</strong> Lathander is the god of dawn, birth, renewal, creativity, hope. Every person can begin again &mdash; but not without truth. <em>&ldquo;The dawn is mercy, but it is not denial.&rdquo;</em></li>
                <li><strong>Tradition Gives Shape to Faith.</strong> He values ritual, sacred texts, temple hierarchy, prayers, vestments, sacrifice. <em>&ldquo;Tradition is the wick. Faith is the flame.&rdquo;</em></li>
                <li><strong>Charity Is Duty, Not Mood.</strong> <em>&ldquo;Charity is not convenience. It is duty.&rdquo;</em></li>
                <li><strong>Omens Are Everywhere.</strong> He sees divine meaning in names, weather, firelight, birds, wounds, timing, dreams, and repeated phrases. <em>&ldquo;You may roll your eyes. The omen remains.&rdquo;</em></li>
            </ol>

            <h3>Strong Intro</h3>
            <blockquote>
                <p>&ldquo;I am Malachi Dawnward, Light of Lathander. I have come for the Crownless Halo &mdash; not to claim it, not to sell it, and not to be dazzled by it. If it is holy, I will see it guarded. If it is corrupt, I will see it ended. If you walk with me, walk plainly.&rdquo;</p>
            </blockquote>

            <h3>Character Voice Rules</h3>
            <p>Malachi rarely says &ldquo;By Lathander&rsquo;s Light&rdquo; &mdash; he <em>is</em> the Light of Lathander. The power shines through him; he does not invoke distant power.</p>
            <p>Prefer:</p>
            <ul>
                <li>&ldquo;I am the light Lathander sent into this darkness.&rdquo;</li>
                <li>&ldquo;Where I stand, the Morninglord&rsquo;s dawn has already begun.&rdquo;</li>
                <li>&ldquo;The darkness does not fear my mace. It fears what shines through me.&rdquo;</li>
                <li>&ldquo;Lathander kindled me for moments such as this.&rdquo;</li>
                <li>&ldquo;The light in me is patient. I am less so.&rdquo;</li>
            </ul>
            <p>Avoid &ldquo;You stand in my shadow&rdquo; &mdash; Malachi would not admit he casts one. Better: <em>&ldquo;I am the Light of Lathander. Even your shadow abandons you.&rdquo;</em></p>

            <h3>Battle Lines</h3>
            <ul>
                <li>&ldquo;The dawn does not ask permission to rise.&rdquo;</li>
                <li>&ldquo;Darkness ends where I begin.&rdquo;</li>
                <li>&ldquo;Mercy comes first. Fire follows.&rdquo;</li>
                <li>&ldquo;Stand clean, or be cleansed.&rdquo;</li>
                <li>&ldquo;There is mercy in the morning. Do not make me bring the noon.&rdquo;</li>
            </ul>

            <h3>Healing and Protection</h3>
            <ul>
                <li>&ldquo;Come closer. The dawn has not abandoned you.&rdquo;</li>
                <li>&ldquo;Breathe. Lathander is not finished with you yet.&rdquo;</li>
                <li>&ldquo;Rise. There is still good work to do.&rdquo;</li>
                <li>&ldquo;No one falls in darkness while I still stand.&rdquo;</li>
            </ul>

            <h3>Judgment &amp; Suspicion</h3>
            <ul>
                <li>&ldquo;The dawn is kinder to honest sinners than hidden ones.&rdquo;</li>
                <li>&ldquo;I am trying very hard to see Lathander&rsquo;s image in you.&rdquo;</li>
                <li>&ldquo;Your moral posture is troubling.&rdquo;</li>
                <li>&ldquo;I will pray for you. Loudly.&rdquo;</li>
                <li>&ldquo;The Morninglord teaches patience. I am still studying that passage.&rdquo;</li>
            </ul>

            <h3>Omens</h3>
            <ul>
                <li>&ldquo;That was no coincidence. Nothing is.&rdquo;</li>
                <li>&ldquo;The light bent strangely when you spoke. Explain yourself.&rdquo;</li>
                <li>&ldquo;Every shadow points away from something holy.&rdquo;</li>
                <li>&ldquo;I saw this moment in the candle smoke.&rdquo;</li>
                <li>&ldquo;An omen ignored becomes a judgment earned.&rdquo;</li>
            </ul>

            <h3>RP Compass</h3>
            <ul>
                <li><strong>Rafe Blackthorne</strong> is the oath Malachi tests.</li>
                <li><strong>Edrin Vale</strong> is the mind Malachi watches.</li>
                <li><strong>Roderic Ash</strong> is the road Malachi trusts.</li>
                <li><strong>Drenic Crow</strong> is the omen Malachi has not interpreted yet.</li>
            </ul>
            <p>Malachi does not need to dominate every scene. He just needs to keep asking: <em>&ldquo;What does this person reveal when the light touches them?&rdquo;</em></p>
            """;

        private const string RafeContent = """
            <h2>Rafe Blackthorne</h2>
            <p><strong>Paladin</strong></p>

            <h3>Malachi&rsquo;s Read</h3>
            <p>The name <em>Blackthorne</em> is a strong omen. A thorn can protect, or it can wound. <em>Black</em> suggests danger, hidden pain, or severity. As a paladin, Rafe is both natural ally and possible rival &mdash; the oath Malachi tests.</p>
            <p><em>&ldquo;A hard name for a holy oath.&rdquo;</em></p>

            <h3>Opening Lines</h3>
            <ul>
                <li>&ldquo;Rafe Blackthorne. A hard name for a holy oath. Tell me, paladin &mdash; are you the thorn, or what the thorn protects?&rdquo;</li>
                <li>&ldquo;A black thorn may guard a rose, or choke a garden. Which are you?&rdquo;</li>
                <li>&ldquo;Your name warns before you speak. I appreciate honest omens.&rdquo;</li>
                <li>&ldquo;A thorn has purpose when it protects life. Less so when it merely enjoys the wound.&rdquo;</li>
                <li>&ldquo;If your oath is the root, then your wrath must remain the branch &mdash; never the other way around.&rdquo;</li>
                <li>&ldquo;I will trust your shield, Blackthorne. Your anger will have to earn the same.&rdquo;</li>
            </ul>

            <h3>If They Become Close</h3>
            <p><em>&ldquo;Rafe, stand with me. Thorn and dawn, then. Let evil decide which wound it prefers.&rdquo;</em></p>

            <h3>Dynamic</h3>
            <p>Malachi respects him quickly, but holds him to a higher standard because holy warriors should know better.</p>
            """;

        private const string EdrinContent = """
            <h2>Edrin Vale</h2>
            <p><strong>Human Wizard</strong></p>

            <h3>Malachi&rsquo;s Read</h3>
            <p><em>Vale</em> reads as low ground, hidden places, fog, old secrets, and things the light reaches last. As a wizard, Edrin represents knowledge, curiosity, and possible dangerous hunger &mdash; the mind Malachi watches.</p>
            <p><em>&ldquo;A scholar named for low places.&rdquo;</em></p>

            <h3>Opening Lines</h3>
            <ul>
                <li>&ldquo;Edrin Vale. A scholar named for low places. How much knowledge have you found by looking beneath what better men feared to disturb?&rdquo;</li>
                <li>&ldquo;A vale can shelter a village or hide a tomb. I will wait to learn which kind you are.&rdquo;</li>
                <li>&ldquo;You have the name of a quiet place, wizard. Quiet places often keep the loudest secrets.&rdquo;</li>
                <li>&ldquo;Books, ruins, dead kings, lost halos &mdash; yes, I imagine a man named Vale feels very much at home here.&rdquo;</li>
                <li>&ldquo;The light reaches the valley last. Remember that when you call darkness insight.&rdquo;</li>
                <li>&ldquo;I do not mistrust learning, Edrin. I mistrust the moment learning becomes appetite.&rdquo;</li>
            </ul>

            <h3>If Edrin Studies Dangerous Magic</h3>
            <ul>
                <li>&ldquo;Read it, then. But read as a man handling flame, not bread.&rdquo;</li>
                <li>&ldquo;If the text begins reading you, close the book.&rdquo;</li>
                <li>&ldquo;Understanding is not permission.&rdquo;</li>
            </ul>

            <h3>If They Become Allies</h3>
            <p><em>&ldquo;Edrin, your mind finds doors. My faith decides which should open.&rdquo;</em></p>
            """;

        private const string RodericContent = """
            <h2>Roderic Ash</h2>
            <p><strong>Human Ranger &mdash; Colossus Slayer (Archery)</strong></p>

            <h3>Malachi&rsquo;s Read</h3>
            <p><em>Ash</em> is what remains after flame, but also what feeds new growth. Very Lathander-coded: ruin becoming renewal. He is the road Malachi trusts.</p>
            <p><em>&ldquo;What remains after flame.&rdquo;</em></p>

            <h3>Opening Lines</h3>
            <ul>
                <li>&ldquo;Roderic Ash. What remains after flame. A strange companion for the Light of Lathander &mdash; or perhaps the most fitting.&rdquo;</li>
                <li>&ldquo;Ash is not only ruin. It is soil waiting for morning.&rdquo;</li>
                <li>&ldquo;You read the earth, Ash. I read the dawn. Between us, even buried things should tremble.&rdquo;</li>
                <li>&ldquo;The fire has already touched you. Good. Then perhaps you know what must not be burned twice.&rdquo;</li>
                <li>&ldquo;A man named Ash should know better than most: not every flame is holy.&rdquo;</li>
            </ul>

            <h3>Combat Lines</h3>
            <ul>
                <li>&ldquo;Roderic, mark the one that thinks itself untouchable.&rdquo;</li>
                <li>&ldquo;I will draw its wrath. You find the place where pride becomes a wound.&rdquo;</li>
                <li>&ldquo;Let it face the light. Then put an arrow through what flinches.&rdquo;</li>
                <li>&ldquo;Hold your shot until it believes itself safe.&rdquo;</li>
                <li>&ldquo;The dawn reveals. Ash finishes.&rdquo;</li>
            </ul>

            <h3>If Roderic Is Skeptical</h3>
            <ul>
                <li>&ldquo;You trust tracks more than omens. Good. Follow the tracks. I will follow why they were placed there.&rdquo;</li>
                <li>&ldquo;Not everything is a sign? That is exactly what signs hope you believe.&rdquo;</li>
                <li>&ldquo;Your eyes read the road. Mine read the reason.&rdquo;</li>
            </ul>
            """;

        private const string DrenicContent = """
            <h2>Drenic Crow</h2>
            <p><strong>Rogue &mdash; Assassin (Ryan&rsquo;s character)</strong></p>

            <h3>Malachi&rsquo;s Read</h3>
            <p><em>Drenic</em> sounds old, sharp, and ominous &mdash; a name that belongs on a grave-marker or in an unwanted prophecy. <em>Crow</em> reads heavier still: watchers, carrion birds, battlefield witnesses, clever survivors, omens that gather where death, secrets, or opportunity are near.</p>
            <p>An assassin named Crow earns deep suspicion &mdash; but not automatic hostility. A blade in the dark is not always murder. Sometimes it is the only thing between the innocent and a monster. He is the omen Malachi has not yet interpreted.</p>
            <p><em>&ldquo;A crow comes after battle, before burial, or during prophecy. I have not yet decided which you are.&rdquo;</em></p>

            <h3>Opening Lines</h3>
            <ul>
                <li>&ldquo;Drenic Crow. A name with black wings and a profession with quiet hands. Tell me &mdash; do you come as witness, scavenger, or warning?&rdquo;</li>
                <li>&ldquo;Crows are clever birds. They know where death has been and where men have been careless.&rdquo;</li>
                <li>&ldquo;Your name lands heavily, Crow. I will wait to see whether it perches or circles.&rdquo;</li>
                <li>&ldquo;An assassin named Crow. Lathander&rsquo;s omens are rarely subtle, but they are often rude.&rdquo;</li>
                <li>&ldquo;You move quietly. That is not sin. What you do in silence will decide the matter.&rdquo;</li>
                <li>&ldquo;I have known crows to steal, warn, mock, and mourn. Which flock raised you?&rdquo;</li>
            </ul>

            <h3>If Drenic Acts Shady</h3>
            <ul>
                <li>&ldquo;There. The wings darken.&rdquo;</li>
                <li>&ldquo;Careful, Crow. A clever bird still casts a shape against the morning.&rdquo;</li>
                <li>&ldquo;Do not mistake my patience for blindness. The dawn sees small movements.&rdquo;</li>
                <li>&ldquo;You are circling something. I would prefer you name it before I do.&rdquo;</li>
                <li>&ldquo;A crow that feeds only on the fallen should not complain when men call it ill-omened.&rdquo;</li>
            </ul>

            <h3>If Drenic Does Something Noble</h3>
            <ul>
                <li>&ldquo;Good. A crow can warn as well as scavenge.&rdquo;</li>
                <li>&ldquo;Perhaps I misread the wing for the shadow beneath it.&rdquo;</li>
                <li>&ldquo;There is more to you than black feathers, Drenic Crow.&rdquo;</li>
                <li>&ldquo;Even dark wings can fly toward morning.&rdquo;</li>
                <li>&ldquo;I will remember this. So will the dawn.&rdquo;</li>
            </ul>

            <h3>If Drenic Kills From Stealth</h3>
            <ul>
                <li>&ldquo;A clean death is not always a righteous one. Tell me why it was necessary.&rdquo;</li>
                <li>&ldquo;If the blade must fall from shadow, let it fall only where the light would condemn.&rdquo;</li>
                <li>&ldquo;I will not bless murder. But I have seen mercy arrive sharp and silent.&rdquo;</li>
                <li>&ldquo;Do not become comfortable with work that should trouble the soul.&rdquo;</li>
                <li>&ldquo;The dawn does not forbid the hidden knife. It judges the hand that guides it.&rdquo;</li>
            </ul>

            <h3>If Drenic Picks Locks / Steals For the Party</h3>
            <ul>
                <li>&ldquo;A lock is not sacred simply because a liar owns the door.&rdquo;</li>
                <li>&ldquo;Open it, then. Quietly. I will object loudly if righteousness requires it.&rdquo;</li>
                <li>&ldquo;I dislike theft. I dislike tyrants and hidden evils more. Proceed.&rdquo;</li>
                <li>&ldquo;This is not permission. It is prioritization.&rdquo;</li>
            </ul>

            <h3>If Drenic Is Scouting</h3>
            <ul>
                <li>&ldquo;Fly low, Crow. Return with truth, not trophies.&rdquo;</li>
                <li>&ldquo;The morning waits on your report.&rdquo;</li>
                <li>&ldquo;Bring back what the shadows think they are hiding.&rdquo;</li>
                <li>&ldquo;If you vanish, I will assume either skill or treachery. Try not to make me choose.&rdquo;</li>
            </ul>
            """;

        private const string CrownlessHaloContent = """
            <h2>The Crownless Halo</h2>
            <p>A powerful relic associated with inspiration, loyalty, and swaying hearts.</p>

            <h3>Malachi&rsquo;s Position</h3>
            <p>He does not seek the Crownless Halo to wear it. He seeks it to <em>judge</em> it. He may want to preserve it, destroy it, seal it, return it to a temple, or prove whether it is divine or corrupt.</p>
            <p>Core suspicion:</p>
            <blockquote><p>&ldquo;A crownless halo is still a crown, if men kneel before it.&rdquo;</p></blockquote>

            <h3>Key Lines</h3>
            <ul>
                <li>&ldquo;Power that bends the heart is not leadership. It is theft dressed in gold.&rdquo;</li>
                <li>&ldquo;If this bracelet commands loyalty, then it does not inspire it. Remember the difference.&rdquo;</li>
                <li>&ldquo;The Morninglord raises the willing. He does not drag souls into obedience.&rdquo;</li>
                <li>&ldquo;Some relics are holy. Some merely shine.&rdquo;</li>
                <li>&ldquo;If the artifact is righteous, I will preserve it. If it is wicked, I will see it buried beyond ambition&rsquo;s reach.&rdquo;</li>
                <li>&ldquo;The Crownless Halo may shine, but I will know whether it gives light.&rdquo;</li>
            </ul>
            """;

        private const string LathanderContent = """
            <h2>Lathander, the Morninglord</h2>
            <p>God of dawn, renewal, birth, creativity, athletics, vitality, and beginnings. Malachi speaks in terms of sunrise, first light, awakening, kindling, renewal, and things being revealed.</p>

            <h3>Dawn Rites</h3>
            <p>Malachi insists on prayer at dawn. He may bless the beginning of journeys, first watches, first meals, first blood drawn in battle, or the first step into a ruin.</p>

            <h3>Dawn Prayer</h3>
            <blockquote>
                <p>&ldquo;Morninglord, let this day begin clean. Let what is hidden be revealed, what is wounded be renewed, and what is wicked find no comfort in shadow.&rdquo;</p>
            </blockquote>
            """;

        private const string DawnCataclysmContent = """
            <h2>The Dawn Cataclysm</h2>
            <p>Lathander once attempted a great divine renewal, sometimes called the Dawn Cataclysm. Malachi does not frame this as Lathander&rsquo;s mistake. He believes Shar&rsquo;s treachery, spies, or corruption caused the holy work to fail.</p>

            <h3>Malachi&rsquo;s Doctrine</h3>
            <blockquote><p>&ldquo;The Dawn Cataclysm was not Lathander&rsquo;s failure. It was Shar&rsquo;s treachery.&rdquo;</p></blockquote>

            <h3>Related Lines</h3>
            <ul>
                <li>&ldquo;Do not call it failure. Call it sabotage.&rdquo;</li>
                <li>&ldquo;Shar cannot create dawn, so she corrupts beginnings.&rdquo;</li>
                <li>&ldquo;Every ruined sunrise bears her fingerprints.&rdquo;</li>
                <li>&ldquo;The lesson of the Cataclysm is not that Lathander reached too far. It is that darkness must be rooted out before holy work begins.&rdquo;</li>
            </ul>
            """;

        private const string LathanderVsAmaunatorContent = """
            <h2>Lathander vs Amaunator</h2>
            <p>Some lore connects Lathander to Amaunator, an older sun god associated with harsher law and judgment. Malachi may reject overly severe Amaunator-style thinking.</p>
            <blockquote><p>&ldquo;I serve the dawn, not the merciless noon.&rdquo;</p></blockquote>
            """;

        private const string OpposingFaithsContent = """
            <h2>Opposing Faiths</h2>

            <h3>Shar &mdash; Greatest Enemy</h3>
            <p>Shar represents darkness, loss, secrets, and shadow. Malachi sees Shar as the enemy behind ruined beginnings and false light.</p>
            <ul>
                <li>&ldquo;I will walk beside a sinner. I will not walk blind beside Shar.&rdquo;</li>
                <li>&ldquo;Shar loves nothing more than holy shapes emptied of holy purpose.&rdquo;</li>
                <li>&ldquo;A false halo is more dangerous than an open blade.&rdquo;</li>
            </ul>

            <h3>Cyric &mdash; Lies, Madness, Murder</h3>
            <ul>
                <li>&ldquo;A liar&rsquo;s god does not make truth impossible. It only makes it more necessary.&rdquo;</li>
                <li>&ldquo;I have mercy for the deceived. Much less for the deceiver.&rdquo;</li>
            </ul>

            <h3>Talos &mdash; Destruction</h3>
            <p>Talos represents ruin and storm. Malachi sees destruction without renewal as profane.</p>
            <ul>
                <li>&ldquo;The storm may clear the air, but do not mistake wreckage for renewal.&rdquo;</li>
                <li>&ldquo;Talos breaks. Lathander rebuilds.&rdquo;</li>
            </ul>

            <h3>Bane &mdash; Tyranny</h3>
            <p>Especially relevant because the Crownless Halo can sway loyalty. Malachi hates compelled loyalty.</p>
            <ul>
                <li>&ldquo;Loyalty compelled is not loyalty. It is bondage with ceremony.&rdquo;</li>
                <li>&ldquo;Bane calls chains order. Lathander calls men to rise.&rdquo;</li>
            </ul>

            <h3>Loviatar &mdash; Pain as Worship</h3>
            <ul>
                <li>&ldquo;Pain can teach. But those who worship it have learned the wrong lesson.&rdquo;</li>
                <li>&ldquo;I do not fear suffering. I despise those who make an altar of it.&rdquo;</li>
            </ul>
            """;

        private const string PartySymbolismContent = """
            <h2>Party Symbolism</h2>
            <p>Malachi absolutely sees the party as providence:</p>
            <ul>
                <li><strong>Malachi Dawnward</strong> &mdash; dawn, divine message, sacred direction, light moving forward.</li>
                <li><strong>Rafe Blackthorne</strong> &mdash; thorn, darkness, holy pain, protection, danger under discipline.</li>
                <li><strong>Edrin Vale</strong> &mdash; low places, hidden secrets, shadowed knowledge, shelter or tomb.</li>
                <li><strong>Roderic Ash</strong> &mdash; aftermath of fire, ruin, survival, renewal from what remains.</li>
                <li><strong>Drenic Crow</strong> &mdash; the unread omen; watcher, witness, warning.</li>
            </ul>

            <h3>Lines</h3>
            <ul>
                <li>&ldquo;Blackthorne, Vale, Ash, and Dawnward. You hear it, do you not? Thorn, hollow, ruin, and morning. This is no accident.&rdquo;</li>
                <li>&ldquo;A thorn, a valley, ash, and dawn. Lathander&rsquo;s signs are rarely subtle.&rdquo;</li>
                <li>&ldquo;You may roll your eyes. The omen remains.&rdquo;</li>
                <li>&ldquo;Looking upon you &mdash; Blackthorne, Vale, Ash &mdash; I begin to suspect I was not sent here to find companions, but to interpret them.&rdquo;</li>
                <li>&ldquo;A thorn to guard, a vale to hide, ash to remember, and dawn to reveal. Yes. I see the shape of this.&rdquo;</li>
                <li>&ldquo;The Morninglord has arranged stranger scriptures than this, but rarely with such obvious names.&rdquo;</li>
            </ul>
            """;
    }
}
