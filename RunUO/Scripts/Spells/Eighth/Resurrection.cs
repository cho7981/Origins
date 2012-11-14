using System;
using Server.Targeting;
using Server.Network;
using Server.Gumps;
using Server.Menus.Questions;
using Server.Mobiles;

namespace Server.Spells.Eighth
{
	public class ResurrectionSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Resurrection", "An Corp",
				245,
				9062,
				Reagent.Bloodmoss,
				Reagent.Garlic,
				Reagent.Ginseng
			);

		public override SpellCircle Circle { get { return SpellCircle.Eighth; } }

		public ResurrectionSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
                Caster.SendAsciiMessage("Target can not be seen.");
				//Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( m == Caster )
			{
                Caster.SendAsciiMessage("Thou can not resurrect thyself.");
				//Caster.SendLocalizedMessage( 501039 ); // Thou can not resurrect thyself.
			}
			else if ( !Caster.Alive )
			{
                Caster.SendAsciiMessage("The resurrecter must be alive.");
				//Caster.SendLocalizedMessage( 501040 ); // The resurrecter must be alive.
			}
			else if ( m.Alive )
			{
                Caster.SendAsciiMessage("Target is not dead.");
				//Caster.SendLocalizedMessage( 501041 ); // Target is not dead.
			}
			else if ( !Caster.InRange( m, 1 ) )
			{
                Caster.SendAsciiMessage("Target is not close enough.");
				//Caster.SendLocalizedMessage( 501042 ); // Target is not close enough.
			}
			else if ( !m.Player )
			{
                Caster.SendAsciiMessage("Target is not a being.");
				//Caster.SendLocalizedMessage( 501043 ); // Target is not a being.
			}
			else if ( m.Map == null || !m.Map.CanFit( m.Location, 16, false, false ) )
			{
                Caster.SendAsciiMessage("Target can not be resurrected at that location.");
                m.SendAsciiMessage("Thou can not be resurrected there!");
				//Caster.SendLocalizedMessage( 501042 ); // Target can not be resurrected at that location.
				//m.SendLocalizedMessage( 502391 ); // Thou can not be resurrected there!
			}
			else if ( m.Region != null && m.Region.IsPartOf( "Khaldun" ) )
			{
				Caster.SendLocalizedMessage( 1010395 ); // The veil of death in this area is too strong and resists thy efforts to restore life.
			}
			else if ( CheckBSequence( m, true ) )
			{
				SpellHelper.Turn( Caster, m );

				m.PlaySound( 0x214 );
				m.FixedEffect( 0x376A, 10, 16 );

                m.CantWalk = true;
				m.CloseGump( typeof( ResurrectGump ) );

                if (m is PlayerMobile && !((PlayerMobile)m).HasMenu)
                {
                    ((PlayerMobile)m).HasMenu = true;
                    m.SendMenu(new ResurrectGump(m, Caster));
                }
				//m.SendGump( new ResurrectGump( m, Caster ) );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private ResurrectionSpell m_Owner;

			public InternalTarget( ResurrectionSpell owner ) : base( 1, false, TargetFlags.Beneficial )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
				{
					m_Owner.Target( (Mobile)o );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}