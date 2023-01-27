Er zijn 2 manieren om een sudoku aan het programma mee te geven.
De eerste manier is om in het bestand Arguments.txt, 81 ints te plaatsen. Er wordt voor de correctheid van het programma vanuit gegaan dat het er exact 81 zijn.
Om dit te laten werken, moet het bestand Arguments.txt gekopieerd worden naar de output directory. Wij hebben dit als het goed is in de solution opgenomen, maar mocht het niet werken:
Rechtermuisknop op Arguments.txt -> Quick Properties -> Copy to output directory.
Als dit tekstbestand leeg is, kunnen er ook 81 ints meegegeven worden via de command line. Ook hier wordt er vanuit gegaan dat het er exact 81 zijn.

Om het algoritme uit te voeren, wordt doCBT aangeroepen in de main functie. Het eerst argument zijn de argumenten die mogelijk via de console worden meegegeven,
en het 2e argument is of je forward checking wilt uitvoeren.
