using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace FPU
{
    public class CommentGenerator
    {
        List<string> registers = new List<string>();
        public int Count { get { return registers.Count; } }
        public Regex rx = new Regex(@"(?m)^\s*(?'instruction'\w+)(?'dest'[\s\w\(01234567\)\+\*\[\]]+)?(,(?'src'[\s\w\(01234567\)\+\*\[\]]+))?;?(?'comment'.*)$", RegexOptions.Compiled);
        //
        public string this[int pos]
        {
            get
            { 
                return registers[Count - 1 - pos];
            }
            set
            {
                registers[Count - 1 - pos] = value;
            }
        }

        public void Init()
        {
            registers.Clear();
        }

        public void fld(string v)
        {
            if (Count >= 8) throw (new Exception("Stack Full"));
            registers.Add(v);
        }

        public void fld(int pos)
        {
            if (Count >= 8) throw (new Exception("Stack Full"));
            registers.Add(this[pos]);
        }

        public string fstp()
        {
            if(Count==0) throw(new Exception("Stack Empty"));
            string v = registers[Count - 1];
            registers.RemoveAt(Count-1);
            return v;
        }
        //---------------------------------------
        public void fadd(string src)
        {
            this[0] = add(this[0], src);
        }

        public void fadd(int dest, int src)
        {
            if (!(dest == 0 || src == 0)) throw (new Exception("invalid parameters"));
            this[dest] = add(this[dest], this[src]);
        }

        public void faddp(int dest, int src)
        {
            if (src != 0) throw (new Exception("invalid parameters"));
            this[dest] = add(this[dest], this[src]);
            fstp();
        }
        //--------------------------------------------------
        public void fsub(string src)
        {
            this[0] = sub(this[0], src);
        }

        public void fsub(int dest, int src)
        {
            if (!(dest == 0 || src == 0)) throw (new Exception("invalid parameters"));
            this[dest] = sub(this[dest], this[src]);
        }

        public void fsubp(int dest, int src)
        {
            if (src != 0) throw (new Exception("invalid parameters"));
            this[dest] = sub(this[dest], this[src]);
            fstp();
        }

        public void fsubr(int dest, int src)
        {
            if (!(dest == 0 || src == 0)) throw (new Exception("invalid parameters"));
            this[dest] = sub(this[src], this[dest]);
        }

        public void fsubr(string src)
        {
            this[0] = sub(src, this[0]);
        }

        public void fsubrp(int dest, int src)
        {
            if (src != 0) throw (new Exception("invalid parameters"));
            this[dest] = sub(this[src], this[dest]);
            fstp();
        }
        //--------------------------------------------------
        public void fmul(int dest, int src)
        {
            if (!(dest == 0 || src == 0)) throw (new Exception("invalid parameters"));
            this[dest] = mul(this[dest], this[src]);
        }

        public void fmul(string src)
        {
            this[0] = mul(this[0],src);
        }

        public void fmulp(int dest, int src)
        {
            if (src != 0) throw (new Exception("invalid parameters"));
            this[dest] = mul(this[dest], this[src]);
            fstp();
        }

        public void fdiv(int dest, int src)
        {
            if (!(dest == 0 || src == 0)) throw (new Exception("invalid parameters"));
            this[dest] = div(this[dest], this[src]);
        }

        public void fdiv(string src)
        {
            this[0] = div(this[0], src);
        }

        public void fdivp(int dest, int src)
        {
            if (src != 0) throw (new Exception("invalid parameters"));
            this[dest] = div(this[dest], this[src]);
            fstp();
        }

        public void fdivr(int dest, int src)
        {
            if (!(dest == 0 || src == 0)) throw (new Exception("invalid parameters"));
            this[dest] = div(this[src], this[dest]);
        }

        public void fdivr(string src)
        {
           this[0] = div(src, this[0]);
        }

        public void fdivrp(int dest, int src)
        {
            if (src != 0) throw (new Exception("invalid parameters"));
            this[dest] = div(this[src], this[dest]);
            fstp();
        }
        //--------------------------------------------------

        public void ffree(int pos)
        {
            this[pos] = "#";
            if (pos==(Count-1))
                registers.RemoveAt(0);
        }

        public void fabs()
        {
            this[0] = "abs(" + this[0] + ")";
        }

        public void fchs()
        {
            string v = this[0];
            int pos_skobka = v.IndexOf("(");
            int pos_minus = v.IndexOf("-");
            int pos_plus = v.IndexOf("+");

            if (pos_minus < 0 && pos_plus < 0)
            {
                this[0] = "-" + v;
            }
            else
            this[0] = "-(" + this[0] + ")";
        }


        public void fsqrt()
        {
            this[0] = "sqrt(" + this[0] + ")";
        }

        public void fcos()
        {
            this[0] = "cos(" + this[0] + ")";
        }

        public void fsin()
        {
            this[0] = "sin(" + this[0] + ")";
        }

        public void fsincos()
        {
            string v = this[0];
            this[0] = "sin(" + v + ")";
            fld("cos(" + v + ")");
        }

        public void fpatan()
        {
            string y = fstp();
            string x = fstp();
            fld("atan(" + x+"," +y+ ")");
        }

        //--------------------------------------------------

        public void fxch(int dest)
        {
            if (dest == -1) dest = 1;
            string t = this[0];
            this[0] = this[dest];
            this[dest] = t;
        }
        //--------------------------------------------------
        
        //--------------------------------------------------
        private string add(string a, string b)
        {
            int pos_skobka = b.IndexOf("(");
            int pos_minus = b.IndexOf("-");
            if (pos_minus < 0)
                return a + "+" + b;
            if (pos_skobka > 0 && pos_skobka < pos_minus)
                return a + "+" + b;
            return a + "+(" + b + ")";
        }

        private string sub(string a, string b)
        {
            int pos_skobka = b.IndexOf("(");
            int pos_plusminus = b.IndexOf("-");
            if (pos_plusminus < 0) pos_plusminus = b.IndexOf("+");
            if (pos_plusminus < 0)
                return a + "-" + b;
            //if (pos_skobka > 0 && pos_skobka < pos_plusminus)
            //    return a + "-" + b;
            return a + "-(" + b + ")";
        }

        private string mul(string a, string b)
        {
            return need_skobka(a) + "*" + need_skobka(b);
        }

        private string div(string a, string b)
        {
            return need_skobka(a) + "/" + need_skobka(b);
        }

        private string need_skobka(string v)
        {
            int pos_skobka = v.IndexOf("(");
            int pos_minus = v.IndexOf("-");
            int pos_plus = v.IndexOf("+");

            if (pos_minus < 0 && pos_plus < 0) return v;
            if (pos_skobka < 0)
            {
                if (pos_minus > -1 || pos_plus > -1) return "(" + v + ")";
            }
            else
            {
                if (pos_minus < pos_skobka || pos_plus < pos_skobka) return "(" + v + ")";
            }
            return v;
        }

        //--------------------------------------------------
        //--------------------------------------------------
        //--------------------------------------------------
        public void process(string instr, string dest, string src, string comment)
        {
            int rd = ParseRegister(dest);
            int rs = ParseRegister(src);

            if (rd > Count || rs > Count) throw new Exception("Out of stack");

            switch (instr)
            {
                case "finit":
                case "fninit": Init(); break;


                case "fld":
                case "fild":
                    if (dest.IndexOf("st(") > -1)
                        fld(rd);
                    else
                        fld(dest);
                    break;
                case "fld1": fld("1"); break;
                case "fldz": fld("0"); break;
                case "fldpi": fld("pi"); break;
                case "fld2e": fld("log2(e)"); break;
                case "fld2t": fld("log2(10)"); break;
                case "fldlg2": fld("log10(2)"); break;
                case "fldln2": fld("ln(2)"); break;

                case "fstp":
                case "fcomp":
                case "fucomp":
                    fstp(); break;

                case "fcompp":
                case "fucompp":
                    fstp(); fstp(); break;

                case "fadd":
                    if (rd == -1)
                        fadd(dest);
                    else
                        fadd(rd, rs);
                    break;
                case "faddp": faddp(rd, rs); break;
                case "fsub":
                    if (rd == -1)
                        fsub(dest);
                    else
                        fsub(rd, rs);
                    break;
                case "fsubr":
                    if (rd == -1)
                        fsubr(dest);
                    else
                        fsubr(rd, rs);
                    break;
                case "fsubp": fsubp(rd, rs); break;
                case "fsubrp": fsubrp(rd, rs); break;
                case "fmul":
                    if (rd == -1)
                        fmul(dest);
                    else
                        fmul(rd, rs);
                    break;
                case "fmulp": fmulp(rd, rs); break;
                case "fdiv":
                    if (rd == -1)
                        fdiv(dest);
                    else
                        fdiv(rd, rs);
                    break;
                case "fdivr":
                    if (rd == -1)
                        fdivr(dest);
                    else
                        fdivr(rd, rs);
                    break;
                case "fdivp": fdivp(rd, rs); break;
                case "fdivrp": fdivrp(rd, rs); break;
                case "fsin": fsin(); break;
                case "fcos": fcos(); break;
                case "fsincos": fsincos(); break;
                case "fsqrt": fsqrt(); break;
                case "fpatan": fpatan(); break;
                case "fxch": fxch(rd); break;
                case "fabs": fabs(); break;
                case "fchs": fchs(); break;
                case "ffree": ffree(rd); break;
                default:
                    break;
            }

            if(comment!="")
            {
                if (instr.EndsWith("p")) rd--;
                if (rd < 0) rd=0;
                this[rd] = comment;
            }
        }

        public int ParseRegister(string register)
        {
            if (register == "st") return 0;
            // st(0)
            if (register.Length < 5) return -1;
            int pos = -1;
            if (!int.TryParse(register.Substring(3, 1), out pos)) pos = -1;
            return pos;
        }

        public string ParseLine(string line)
        {
            if (line == "") return "";
            if (line.StartsWith(";")) return "";
            if (!line.StartsWith("f",StringComparison.CurrentCultureIgnoreCase)) return "";

            Match m = rx.Match(line.ToLower().Replace("qword","").Replace("ptr",""));
            if (!m.Success) return "#error#";            
            try
            {
                string instruction = m.Groups["instruction"].ToString().Trim();
                string dest = m.Groups["dest"].ToString().Trim();
                string src = m.Groups["src"].ToString().Trim();
                string comment = m.Groups["comment"].ToString().Trim();

                if (comment.StartsWith("="))
                {
                    int pos = ParseRegister(dest);
                    if (pos < 0) pos = 0;
                    comment = comment.Substring(1, -1 + comment.Length).TrimStart();
                }
                else comment = "";

                process(instruction, dest, src, comment);     
            }
            catch (Exception e)
            {
                return "#error: " + e.Message;
            }
            
            return
                // m.Groups["instruction"].ToString().Trim() + " " +
                //m.Groups["dest"].ToString().Trim() + ", " +
                //m.Groups["src"].ToString().Trim() + "; " +
                ToString();
        }

        public override string ToString()
        {
            if (Count == 0) return "0~||";
            StringBuilder sb = new StringBuilder();
            sb.Append(Count.ToString() + "~");
            for (int i = 0; i < Count; i++)
            {
                int pos = Count - 1 - i;
                sb.Append("| ");
                //sb.Append(pos.ToString());
               // sb.Append(": ");
                sb.Append(registers[i]);
                sb.Append(" ");
            }
            sb.Append("|");
            return sb.ToString();
        }


    }
}
