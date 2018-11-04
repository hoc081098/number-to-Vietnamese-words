import NumberToVietnamese.convert
import java.text.DecimalFormat

object NumberToVietnamese {
  private val zeroLeftPadding = arrayOf("", "00", "0")
  private val digits = arrayOf(
    "không",
    "một",
    "hai",
    "ba",
    "bốn",
    "năm",
    "sáu",
    "bảy",
    "tám",
    "chín"
  )
  private val multipleThousand = arrayOf(
    "",
    "nghìn",
    "triệu",
    "tỷ",
    "nghìn",
    "triệu",
    "tỷ"
  )

  private fun readTriple(triple: String, showZeroHundred: Boolean): String {
    val (a, b, c) = triple.map { it - '0' }
    return when {
      a == 0 && b == 0 && c == 0 -> ""
      a == 0 && b == 0 -> digits[c]
      a == 0 && showZeroHundred -> "không trăm " + readPair(b, c)
      a == 0 && b != 0 -> readPair(b, c)
      else -> digits[a] + " trăm " + readPair(b, c)
    }
  }

  private fun readPair(b: Int, c: Int): String {
    return when (b) {
      0 -> when (c) {
        0 -> ""
        else -> " lẻ " + digits[c]
      }
      1 -> "mười " + when (c) {
        0 -> ""
        5 -> "lăm"
        else -> digits[c]
      }
      else -> digits[b] + " mươi " + when (c) {
        0 -> ""
        1 -> "mốt"
        4 -> "tư"
        5 -> "lăm"
        else -> digits[c]
      }
    }
  }

  @JvmStatic
  fun convert(n: Long): String {
    return when {
      n == 0L -> "Không"
      n < 0L -> "Âm " + convert(-n)
      else -> {
        val s = n.toString()
        val groups = "${zeroLeftPadding[s.length % 3]}$s".chunked(3)

        groups.foldIndexed("") { index, acc, e ->
          val readTriple = readTriple(e, groups.size > 1)
          "$acc $readTriple ${when {
            readTriple.isNotBlank() || (groups.size >= 5 && groups.size - 1 - index == 3) -> multipleThousand.getOrNull(
              groups.size - 1 - index
            ).orEmpty()
            else -> ""
          }} "
        }
      }
    }.replace("""\s+""".toRegex(), " ")
      .trim()
      .toLowerCase()
      .capitalize()
  }
}

fun main() {
  println(convert(-55))
  println(convert(-1_055))
  println(convert(101_002_101_000_000_000))
  println(convert(100_000_000_000))
  println(convert(1_000_000_000_000))
  println(convert(1_000_000_000_000_000))
  println(convert(1_000_000_000_000_000_000))
  println(convert(1_000_000_234_000_000_000))
  println(convert(1_000_000_000_222_000_000))
  println(DecimalFormat("###,###").format(Long.MAX_VALUE) + " = " + convert(Long.MAX_VALUE))
  (100_000..2_000_000L step 10_000).map { "${DecimalFormat("###,###").format(it)} = ${convert(it)}" }.forEach(::println)
}

/*
Console:
  Âm năm mươi lăm
  Âm một nghìn không trăm năm mươi lăm
  Một trăm lẻ một triệu hai nghìn một trăm lẻ một tỷ
  Một trăm tỷ
  Một nghìn tỷ
  Một triệu tỷ
  Một tỷ tỷ
  Một tỷ hai trăm ba mươi tư tỷ
  Một tỷ tỷ hai trăm hai mươi hai triệu
  9,223,372,036,854,775,807 = Chín tỷ hai trăm hai mươi ba triệu ba trăm bảy mươi hai nghìn không trăm ba mươi sáu tỷ tám trăm năm mươi tư triệu bảy trăm bảy mươi lăm nghìn tám trăm lẻ bảy
  100,000 = Một trăm nghìn
  110,000 = Một trăm mười nghìn
  120,000 = Một trăm hai mươi nghìn
  130,000 = Một trăm ba mươi nghìn
  140,000 = Một trăm bốn mươi nghìn
  150,000 = Một trăm năm mươi nghìn
  160,000 = Một trăm sáu mươi nghìn
  170,000 = Một trăm bảy mươi nghìn
  180,000 = Một trăm tám mươi nghìn
  190,000 = Một trăm chín mươi nghìn
  200,000 = Hai trăm nghìn
  210,000 = Hai trăm mười nghìn
  220,000 = Hai trăm hai mươi nghìn
  230,000 = Hai trăm ba mươi nghìn
  240,000 = Hai trăm bốn mươi nghìn
  250,000 = Hai trăm năm mươi nghìn
  260,000 = Hai trăm sáu mươi nghìn
  270,000 = Hai trăm bảy mươi nghìn
  280,000 = Hai trăm tám mươi nghìn
  290,000 = Hai trăm chín mươi nghìn
  300,000 = Ba trăm nghìn
  310,000 = Ba trăm mười nghìn
  320,000 = Ba trăm hai mươi nghìn
  330,000 = Ba trăm ba mươi nghìn
  340,000 = Ba trăm bốn mươi nghìn
  350,000 = Ba trăm năm mươi nghìn
  360,000 = Ba trăm sáu mươi nghìn
  370,000 = Ba trăm bảy mươi nghìn
  380,000 = Ba trăm tám mươi nghìn
  390,000 = Ba trăm chín mươi nghìn
  400,000 = Bốn trăm nghìn
  410,000 = Bốn trăm mười nghìn
  420,000 = Bốn trăm hai mươi nghìn
  430,000 = Bốn trăm ba mươi nghìn
  440,000 = Bốn trăm bốn mươi nghìn
  450,000 = Bốn trăm năm mươi nghìn
  460,000 = Bốn trăm sáu mươi nghìn
  470,000 = Bốn trăm bảy mươi nghìn
  480,000 = Bốn trăm tám mươi nghìn
  490,000 = Bốn trăm chín mươi nghìn
  500,000 = Năm trăm nghìn
  510,000 = Năm trăm mười nghìn
  520,000 = Năm trăm hai mươi nghìn
  530,000 = Năm trăm ba mươi nghìn
  540,000 = Năm trăm bốn mươi nghìn
  550,000 = Năm trăm năm mươi nghìn
  560,000 = Năm trăm sáu mươi nghìn
  570,000 = Năm trăm bảy mươi nghìn
  580,000 = Năm trăm tám mươi nghìn
  590,000 = Năm trăm chín mươi nghìn
  600,000 = Sáu trăm nghìn
  610,000 = Sáu trăm mười nghìn
  620,000 = Sáu trăm hai mươi nghìn
  630,000 = Sáu trăm ba mươi nghìn
  640,000 = Sáu trăm bốn mươi nghìn
  650,000 = Sáu trăm năm mươi nghìn
  660,000 = Sáu trăm sáu mươi nghìn
  670,000 = Sáu trăm bảy mươi nghìn
  680,000 = Sáu trăm tám mươi nghìn
  690,000 = Sáu trăm chín mươi nghìn
  700,000 = Bảy trăm nghìn
  710,000 = Bảy trăm mười nghìn
  720,000 = Bảy trăm hai mươi nghìn
  730,000 = Bảy trăm ba mươi nghìn
  740,000 = Bảy trăm bốn mươi nghìn
  750,000 = Bảy trăm năm mươi nghìn
  760,000 = Bảy trăm sáu mươi nghìn
  770,000 = Bảy trăm bảy mươi nghìn
  780,000 = Bảy trăm tám mươi nghìn
  790,000 = Bảy trăm chín mươi nghìn
  800,000 = Tám trăm nghìn
  810,000 = Tám trăm mười nghìn
  820,000 = Tám trăm hai mươi nghìn
  830,000 = Tám trăm ba mươi nghìn
  840,000 = Tám trăm bốn mươi nghìn
  850,000 = Tám trăm năm mươi nghìn
  860,000 = Tám trăm sáu mươi nghìn
  870,000 = Tám trăm bảy mươi nghìn
  880,000 = Tám trăm tám mươi nghìn
  890,000 = Tám trăm chín mươi nghìn
  900,000 = Chín trăm nghìn
  910,000 = Chín trăm mười nghìn
  920,000 = Chín trăm hai mươi nghìn
  930,000 = Chín trăm ba mươi nghìn
  940,000 = Chín trăm bốn mươi nghìn
  950,000 = Chín trăm năm mươi nghìn
  960,000 = Chín trăm sáu mươi nghìn
  970,000 = Chín trăm bảy mươi nghìn
  980,000 = Chín trăm tám mươi nghìn
  990,000 = Chín trăm chín mươi nghìn
  1,000,000 = Một triệu
  1,010,000 = Một triệu không trăm mười nghìn
  1,020,000 = Một triệu không trăm hai mươi nghìn
  1,030,000 = Một triệu không trăm ba mươi nghìn
  1,040,000 = Một triệu không trăm bốn mươi nghìn
  1,050,000 = Một triệu không trăm năm mươi nghìn
  1,060,000 = Một triệu không trăm sáu mươi nghìn
  1,070,000 = Một triệu không trăm bảy mươi nghìn
  1,080,000 = Một triệu không trăm tám mươi nghìn
  1,090,000 = Một triệu không trăm chín mươi nghìn
  1,100,000 = Một triệu một trăm nghìn
  1,110,000 = Một triệu một trăm mười nghìn
  1,120,000 = Một triệu một trăm hai mươi nghìn
  1,130,000 = Một triệu một trăm ba mươi nghìn
  1,140,000 = Một triệu một trăm bốn mươi nghìn
  1,150,000 = Một triệu một trăm năm mươi nghìn
  1,160,000 = Một triệu một trăm sáu mươi nghìn
  1,170,000 = Một triệu một trăm bảy mươi nghìn
  1,180,000 = Một triệu một trăm tám mươi nghìn
  1,190,000 = Một triệu một trăm chín mươi nghìn
  1,200,000 = Một triệu hai trăm nghìn
  1,210,000 = Một triệu hai trăm mười nghìn
  1,220,000 = Một triệu hai trăm hai mươi nghìn
  1,230,000 = Một triệu hai trăm ba mươi nghìn
  1,240,000 = Một triệu hai trăm bốn mươi nghìn
  1,250,000 = Một triệu hai trăm năm mươi nghìn
  1,260,000 = Một triệu hai trăm sáu mươi nghìn
  1,270,000 = Một triệu hai trăm bảy mươi nghìn
  1,280,000 = Một triệu hai trăm tám mươi nghìn
  1,290,000 = Một triệu hai trăm chín mươi nghìn
  1,300,000 = Một triệu ba trăm nghìn
  1,310,000 = Một triệu ba trăm mười nghìn
  1,320,000 = Một triệu ba trăm hai mươi nghìn
  1,330,000 = Một triệu ba trăm ba mươi nghìn
  1,340,000 = Một triệu ba trăm bốn mươi nghìn
  1,350,000 = Một triệu ba trăm năm mươi nghìn
  1,360,000 = Một triệu ba trăm sáu mươi nghìn
  1,370,000 = Một triệu ba trăm bảy mươi nghìn
  1,380,000 = Một triệu ba trăm tám mươi nghìn
  1,390,000 = Một triệu ba trăm chín mươi nghìn
  1,400,000 = Một triệu bốn trăm nghìn
  1,410,000 = Một triệu bốn trăm mười nghìn
  1,420,000 = Một triệu bốn trăm hai mươi nghìn
  1,430,000 = Một triệu bốn trăm ba mươi nghìn
  1,440,000 = Một triệu bốn trăm bốn mươi nghìn
  1,450,000 = Một triệu bốn trăm năm mươi nghìn
  1,460,000 = Một triệu bốn trăm sáu mươi nghìn
  1,470,000 = Một triệu bốn trăm bảy mươi nghìn
  1,480,000 = Một triệu bốn trăm tám mươi nghìn
  1,490,000 = Một triệu bốn trăm chín mươi nghìn
  1,500,000 = Một triệu năm trăm nghìn
  1,510,000 = Một triệu năm trăm mười nghìn
  1,520,000 = Một triệu năm trăm hai mươi nghìn
  1,530,000 = Một triệu năm trăm ba mươi nghìn
  1,540,000 = Một triệu năm trăm bốn mươi nghìn
  1,550,000 = Một triệu năm trăm năm mươi nghìn
  1,560,000 = Một triệu năm trăm sáu mươi nghìn
  1,570,000 = Một triệu năm trăm bảy mươi nghìn
  1,580,000 = Một triệu năm trăm tám mươi nghìn
  1,590,000 = Một triệu năm trăm chín mươi nghìn
  1,600,000 = Một triệu sáu trăm nghìn
  1,610,000 = Một triệu sáu trăm mười nghìn
  1,620,000 = Một triệu sáu trăm hai mươi nghìn
  1,630,000 = Một triệu sáu trăm ba mươi nghìn
  1,640,000 = Một triệu sáu trăm bốn mươi nghìn
  1,650,000 = Một triệu sáu trăm năm mươi nghìn
  1,660,000 = Một triệu sáu trăm sáu mươi nghìn
  1,670,000 = Một triệu sáu trăm bảy mươi nghìn
  1,680,000 = Một triệu sáu trăm tám mươi nghìn
  1,690,000 = Một triệu sáu trăm chín mươi nghìn
  1,700,000 = Một triệu bảy trăm nghìn
  1,710,000 = Một triệu bảy trăm mười nghìn
  1,720,000 = Một triệu bảy trăm hai mươi nghìn
  1,730,000 = Một triệu bảy trăm ba mươi nghìn
  1,740,000 = Một triệu bảy trăm bốn mươi nghìn
  1,750,000 = Một triệu bảy trăm năm mươi nghìn
  1,760,000 = Một triệu bảy trăm sáu mươi nghìn
  1,770,000 = Một triệu bảy trăm bảy mươi nghìn
  1,780,000 = Một triệu bảy trăm tám mươi nghìn
  1,790,000 = Một triệu bảy trăm chín mươi nghìn
  1,800,000 = Một triệu tám trăm nghìn
  1,810,000 = Một triệu tám trăm mười nghìn
  1,820,000 = Một triệu tám trăm hai mươi nghìn
  1,830,000 = Một triệu tám trăm ba mươi nghìn
  1,840,000 = Một triệu tám trăm bốn mươi nghìn
  1,850,000 = Một triệu tám trăm năm mươi nghìn
  1,860,000 = Một triệu tám trăm sáu mươi nghìn
  1,870,000 = Một triệu tám trăm bảy mươi nghìn
  1,880,000 = Một triệu tám trăm tám mươi nghìn
  1,890,000 = Một triệu tám trăm chín mươi nghìn
  1,900,000 = Một triệu chín trăm nghìn
  1,910,000 = Một triệu chín trăm mười nghìn
  1,920,000 = Một triệu chín trăm hai mươi nghìn
  1,930,000 = Một triệu chín trăm ba mươi nghìn
  1,940,000 = Một triệu chín trăm bốn mươi nghìn
  1,950,000 = Một triệu chín trăm năm mươi nghìn
  1,960,000 = Một triệu chín trăm sáu mươi nghìn
  1,970,000 = Một triệu chín trăm bảy mươi nghìn
  1,980,000 = Một triệu chín trăm tám mươi nghìn
  1,990,000 = Một triệu chín trăm chín mươi nghìn
  2,000,000 = Hai triệu

  Process finished with exit code 0

 */