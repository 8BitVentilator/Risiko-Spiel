using FluentAssertions;
using Risiko.Core.Kontinente;

namespace Risiko.Core.Tests
{
    public class SpielbrettTests
    {
        [Fact]
        public void StarteSpiel_MitZweiSpielern_SpielBereitschaftUndVerteilungDerLaender()
        {
            var spieler1 = new Spieler("Andr�", Farbe.Blau);
            var spieler2 = new Spieler("Gregor", Farbe.Rot);

            var spielbrett = new Spielbrett();
            spielbrett.StarteSpiel(spieler1, spieler2);

            spielbrett.Spieler.Count.Should().Be(2);
            spielbrett.Spieler.First().Name.Should().Be("Andr�");
            spielbrett.Spieler.First().Farbe.Should().Be(Farbe.Blau);
            spielbrett.Spieler.Last().Name.Should().Be("Gregor");
            spielbrett.Spieler.Last().Farbe.Should().Be(Farbe.Rot);
            spielbrett.Kontinente.Count.Should().Be(6);

            spielbrett.Kontinente.SelectMany(kontinent => kontinent.Laender).Count().Should().Be(42);
            spielbrett.AktiverSpieler.Should().Be(spieler1);

            var spieler1Laender = spielbrett.Kontinente.SelectMany(kontinent => kontinent.Laender)
                .Count(land => land.BesitzerSpielerId == spieler1.Id);
            spieler1Laender.Should().Be(21);

            var spieler2Laender = spielbrett.Kontinente.SelectMany(kontinent => kontinent.Laender)
                .Count(land => land.BesitzerSpielerId == spieler2.Id);
            spieler2Laender.Should().Be(21);
        }

        [Fact]
        public void BeendeSpielzug_NachErstemSpielzug_EinheitenDesZweitenSpielers()
        {
            var spieler1 = new Spieler("Andr�", Farbe.Blau);
            var spieler2 = new Spieler("Gregor", Farbe.Rot);

            var europa = new Europa();
            var no = europa.Laender.Single(x => x.Name == "Nordeuropa");
            var wo = europa.Laender.Single(x => x.Name == "Westeuropa");
            no.SetzeEinheitenUndBesetze(spieler1.Id, 2);
            wo.SetzeEinheitenUndBesetze(spieler2.Id, 2);

            var spielbrett = new Spielbrett();
            spielbrett.LadeSpielstand(new List<Spieler> { spieler1, spieler2 }, new List<Kontinent>{ europa }, spieler1);
            spielbrett.BeendeSpielzug();
            spielbrett.AktiverSpieler.Should().Be(spieler2);
            spieler2.FreieEinheiten.Should().Be(3);
        }

        [Fact]
        public void BeendeSpielzug_NachZweitenSpielzug_EinheitenDesErstenSpielers()
        {
            var spieler1 = new Spieler("Andr�", Farbe.Blau);
            var spieler2 = new Spieler("Gregor", Farbe.Rot);

            var europa = new Europa();
            var no = europa.Laender.Single(x => x.Name == "Nordeuropa");
            var wo = europa.Laender.Single(x => x.Name == "Westeuropa");
            no.SetzeEinheitenUndBesetze(spieler1.Id, 2);
            wo.SetzeEinheitenUndBesetze(spieler2.Id, 2);

            var spielbrett = new Spielbrett();
            spielbrett.LadeSpielstand(new List<Spieler> { spieler1, spieler2 }, new List<Kontinent> { europa }, spieler1);
            spielbrett.BeendeSpielzug();
            spielbrett.BeendeSpielzug();
            spielbrett.AktiverSpieler.Should().Be(spieler1);
            spieler1.FreieEinheiten.Should().Be(3);
        }

        [Fact]
        public void BeendeSpielzug_ZweiterSpielerHatMehrereLaender_EinheitenDesErstenSpielers()
        {
            var spieler1 = new Spieler("Andr�", Farbe.Blau);
            var spieler2 = new Spieler("Gregor", Farbe.Rot);

            var europa = new Europa();
            // Spieler 1 mit 2 L�ndern
            var no = europa.Laender.Single(x => x.Name == "Nordeuropa");
            var gb = europa.Laender.Single(x => x.Name == "Gro�britannien");
            no.SetzeEinheitenUndBesetze(spieler1.Id, 2);
            gb.SetzeEinheitenUndBesetze(spieler1.Id, 2);

            // Spieler 2 mit 1 Land
            var wo = europa.Laender.Single(x => x.Name == "Westeuropa");
            wo.SetzeEinheitenUndBesetze(spieler2.Id, 2);

            var spielbrett = new Spielbrett();
            spielbrett.LadeSpielstand(new List<Spieler> { spieler1, spieler2 }, new List<Kontinent> { europa }, spieler1);
            spielbrett.BeendeSpielzug();
            spielbrett.BeendeSpielzug();
            spielbrett.AktiverSpieler.Should().Be(spieler1);
            spieler1.FreieEinheiten.Should().Be(3);
        }

        [Fact]
        public void BeendeSpielzug_KomplettesKontinentEinemSpielerZugehoerig_FreieEinheitenDesSpielers()
        {
            var spieler1 = new Spieler("Andr�", Farbe.Blau);
            var spieler2 = new Spieler("Gregor", Farbe.Rot);

            var europa = new Europa();

            foreach(var land in europa.Laender)
                land.SetzeEinheitenUndBesetze(spieler1.Id, 1);

            var spielbrett = new Spielbrett();
            spielbrett.LadeSpielstand(new List<Spieler> { spieler1, spieler2 }, new List<Kontinent> { europa }, spieler2);
            spielbrett.BeendeSpielzug();
            spielbrett.AktiverSpieler.Should().Be(spieler1);
            spieler1.FreieEinheiten.Should().Be(8);
        }
    }
}