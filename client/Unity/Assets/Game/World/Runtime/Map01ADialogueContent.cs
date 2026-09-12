using System;

namespace LinhGioi.World
{
    // Local Map01A copy follows reference 05; quest effects remain owned by the map state.
    public static class Map01ADialogueContent
    {
        public static NpcDialogueSession Create(string node, bool offer, bool done, string objective)
        {
            string speaker, accept;
            string[] lines;
            switch (node)
            {
                case "spawn-ha-van":
                    speaker = "Hạ Vân"; accept = "Bắt đầu hành trình";
                    lines = offer ? new[] {
                        "Chào mừng đến Cổng Đông Lâm. Ta là Hạ Vân. Nếu cần chỉ dẫn, cứ tìm ta.",
                        "Phía sau những dãy núi kia là Linh Thành. Trước khi đi xa, hãy làm quen với đường làng và những người ở đây.",
                        "Theo đường đá tới Đại Cổng, ngươi sẽ nhìn rõ Linh Thành. Sau đó hãy gặp Quan Thủ Đông Lâm để hỏi đường." }
                        : new[] { "Ngươi lại ghé rồi. Đường làng đã quen hơn chưa?", "Cứ thong thả chuẩn bị. Khi cần biết việc tiếp theo, ta sẽ nhắc đường cho ngươi." };
                    break;
                case "quan-thu":
                    speaker = "Quan Thủ Đông Lâm"; accept = "Đã hiểu, tôi sẽ chuẩn bị";
                    lines = offer ? new[] {
                        "Ta phụ trách giữ yên Cổng Đông Lâm. Trong làng không được giao chiến; người mới tới cũng phải giữ luật này.",
                        "Qua rìa làng có thú non nhiễm linh khí. Chưa chuẩn bị thì đừng vội tới gần chúng.",
                        "Ghé quảng trường xem hành trang, rồi gặp Tổng Phú nhận tiếp tế. Thanh Nhi và Lão Trần sẽ giúp ngươi chuẩn bị thêm." }
                        : done ? new[] { "Ta đã dặn ngươi luật khu an toàn. Hãy giữ đường làng bình yên cho mọi người.", "Làm xong việc được giao và chuẩn bị trang bị rồi hãy tiếp tục tới lối Suối Thanh Minh." }
                        : new[] { "Ngươi mới đến phải không? Hãy gặp Hạ Vân trước.", "Từ Đại Cổng có thể nhìn thấy Linh Thành. Khi đã nghe chỉ dẫn, quay lại đây, ta sẽ nói rõ luật trong làng." };
                    break;
                case "tong-phu":
                    speaker = "Tổng Phú"; accept = "Nhận tiếp tế";
                    lines = offer ? new[] {
                        "Đi đường xa thì không thể thiếu đồ tiếp tế. Ngươi đã xem hành trang ở quảng trường rồi chứ?",
                        "Ta gửi ngươi ba Bình Máu Nhỏ và hai Bình Linh Lực Nhỏ. Giữ chúng trong hành trang để dùng khi cần.",
                        "Khí sắc ngươi còn yếu. Nhận đồ xong, hãy mở hành trang và dùng một Bình Máu Nhỏ trước khi đi tiếp." }
                        : done ? new[] { "Trông ngươi khỏe hơn rồi đấy. Phần tiếp tế tân thủ ta đã giao đủ.", "Đồ dùng có hạn; kiểm lại hành trang trước mỗi chuyến đi. Thanh Nhi ở dược quán sẽ chỉ ngươi cách tìm linh thảo." }
                        : new[] { "Cứ xem kỹ hành trang trước khi lên đường.", "Nếu đã nhận tiếp tế, hãy dùng một Bình Máu Nhỏ. Phần quà tân thủ chỉ nhận một lần; giữ lại số bình còn lại cho chuyến đi." };
                    break;
                case "thanh-nhi":
                    speaker = "Thanh Nhi"; accept = "Nhận việc hái Linh Thảo";
                    lines = offer ? new[] {
                        "Ta là Thanh Nhi, trông nom dược quán này. Ngươi đã chuẩn bị bình hồi phục chưa?",
                        "Bên giếng có một cụm Linh Thảo Non phát sáng dịu. Hãy tới đó hái; đừng nhầm với Thảo Yêu ở rìa làng.",
                        "Tới gần cụm cây rồi chọn Hái Linh Thảo. Xong việc, tiếp tục theo đường làng gặp Lão Trần." }
                        : done ? new[] { "Ngươi đã tìm đúng Linh Thảo Non rồi. Nhớ sắc lá và ánh sáng ấy nhé.", "Lão Trần đang ở phía trước. Khi cần hồi phục, mở hành trang kiểm số bình còn lại trước khi tới rìa làng." }
                        : new[] { "Linh thảo mọc gần giếng, dọc đường làng phía trước.", "Nếu chưa nhận chỉ dẫn, hãy hoàn tất việc chuẩn bị với Tổng Phú trước. Nếu đã nhận việc, tới gần cây phát sáng và hái nó." };
                    break;
                case "well-bridge":
                    speaker = "Tiểu Đồng"; accept = "Cảm ơn, tôi sẽ xem thử";
                    lines = done ? new[] { "Ngươi tìm được chiếc rương rồi! Ta biết nơi này có điều thú vị mà.", "Cứ theo đường làng tới chỗ Lão Trần. Cẩn thận khi đến rìa làng nhé." }
                        : new[] { "Ta thường trông giếng ở đây. Thanh Nhi có nhờ ngươi tìm Linh Thảo không?", "Hái xong, thử nhìn quanh giếng xem. Có một chiếc rương nhỏ; tới gần rồi chọn Mở rương ẩn.", "Chiếc rương là việc khám phá thêm thôi. Ngươi vẫn có thể tiếp tục việc chính nếu chưa muốn mở." };
                    break;
                case "lao-tran":
                    speaker = "Lão Trần"; accept = "Nhận việc ở rìa làng";
                    lines = offer ? new[] {
                        "Ta vừa từ rìa làng về. Vài con thú non bị linh khí tác động, người mới đi ngang dễ gặp nguy hiểm.",
                        "Ngươi giúp ta hạ một con nhé. Đến gần rồi mới ra đòn; đừng giao chiến trong khu dân cư.",
                        "Hạ xong, nhớ nhặt chiến lợi phẩm và mặc Hộ Uyển Tân Thủ trong hành trang. Chuẩn bị đủ rồi mới đi tiếp tới Suối Thanh Minh." }
                        : done ? new[] { "Ta nghe vùng rìa làng đã yên hơn. Ngươi đã làm tốt việc được giao.", "Đừng bỏ sót chiến lợi phẩm. Mặc món trang bị nhận được và kiểm lại bình hồi phục trước khi tiếp tục." }
                        : new[] { "Đường tới rìa làng ở phía trước, nhưng ngươi cần chuẩn bị đầy đủ.", "Làm theo chỉ dẫn của mọi người trước. Nếu đã nhận việc của ta, hãy hạ một con thú non rồi nhặt chiến lợi phẩm." };
                    break;
                default: throw new ArgumentException("Unknown Map01A NPC: " + node);
            }
            return new NpcDialogueSession(speaker, lines, "Hỏi việc tiếp theo", objective,
                offer ? accept : "Tạm biệt");
        }
    }
}
