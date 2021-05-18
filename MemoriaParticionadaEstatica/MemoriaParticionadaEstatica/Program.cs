using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoriaParticionadaEstatica
{

    class Particion
    {
        private int intTamano;

        public int Tamano
        {
            get { return intTamano; }
            set { intTamano = value; }
        }

        private Trabajo trabajo;

        public Trabajo Trabajo
        {
            get { return trabajo; }
            set { trabajo = value; }
        }

        private Boolean blnSO;

        public Boolean SO
        {
            get { return blnSO; }
            set { blnSO = value; }
        }


        public Particion()
        {
            Tamano = 0;
        }

        public Particion(int intTamano)
        {
            Tamano = intTamano;
        }

        public Particion(int intTamano, Trabajo trabajo)
        {
            Tamano = intTamano;
            Trabajo = trabajo;
        }
        public Particion(Trabajo trabajo)
        {
            Tamano = trabajo.Tamano;
            Trabajo = trabajo;
            SO = true;
        }
    }

    class Trabajo
    {
        private int intNoTrabajo;

        public int NoTrabajo
        {
            get { return intNoTrabajo; }
            set { intNoTrabajo = value; }
        }

        private int intTamano;

        public int Tamano
        {
            get { return intTamano; }
            set { intTamano = value; }
        }

        public Trabajo(int noTrabajo)
        {
            this.NoTrabajo = noTrabajo;
        }

        public Trabajo(int noTrabajo, int tamano)
        {
            this.NoTrabajo = noTrabajo;
            this.Tamano = tamano;
        }
        public virtual bool Equals(int otroTrabajo)
        {
            return intTamano == otroTrabajo;
        }
    }

    class Program
    {
        private static int origRow, origCol;

        static double TamMemoriaN = 0, MemoriaResN = 0, noTrabajoSiguiente = 1, divisor = 20;
        static int unidades;
        static string txtUnidades = "";
        static List<Particion> Particiones; //celda 0 = SO
        static List<Trabajo> StandBy;

        static void Main(string[] args)
        {
            Console.Clear();
            origRow = Console.CursorTop;
            origCol = Console.CursorLeft;

            StandBy = new List<Trabajo>();

            /*Bienvenida...*/
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("//////////////////////////////////////////////////////////////////");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Bienvenido al programa simulador de Memoria Particionada Estática.");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("//////////////////////////////////////////////////////////////////");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("\nPresione cualquier letra para continuar.\n\n");
            Console.ForegroundColor = ConsoleColor.White;
            Console.ReadKey();

            CambiarUnidades();

            PreguntarTamaños();
            PreguntarParticiones();

            PreguntarTrabajos();

        }

        private static void CambiarUnidades()
        {
            while (true)
            {
                Console.WriteLine("Introduzca en qué unidades trabajará\n [1]-KB\n [2]-MB\n [3]-GB");
                try
                {
                    unidades = Int32.Parse(Console.ReadLine());
                    if (unidades > 0 && unidades < 4) break;
                    else throw new Exception();
                }
                catch (Exception)
                {
                    Advertencia("Digito no valido.");
                    Console.Clear();
                }
            }

            if (unidades == 1) txtUnidades = "KB";
            if (unidades == 2) txtUnidades = "MB";
            if (unidades == 3) txtUnidades = "GB";
        }

        private static void PreguntarTamaños()
        {
            Particiones = new List<Particion>();

            try
            {
                /*Pregunta el tamaño total*/
                Console.WriteLine("\n\n¿Cuál es el tamaño total de la memoria (" + txtUnidades + ")?");
                TamMemoriaN = ConvertirUnidadesAKBytes(Int32.Parse(Console.ReadLine()));
                if (TamMemoriaN < 1) throw new Exception();

                //Esta variable ayudará a dividir la memoria gráficada
                divisor = TamMemoriaN / 25;

                double maxSO;
                Trabajo TrabajoSO;

                do
                {
                    /*Pregunta y calcula el tamaño del SO*/
                    maxSO = ConvertirKBytesAUnidades(TamMemoriaN) * .30; //limite de cantidad de memoria que se puede asignar
                    TrabajoSO = new Trabajo(0); //creamos un trabajo 0 que será el SO

                    ActualizarTabla();

                    Console.WriteLine("¿Cuánta memoria desea dedicar al Sistema Operativo (" + txtUnidades + ")? Máximo: " + maxSO + " " + txtUnidades + "");
                    Nota("Introduzca un 0 para cambiar las unidades.");
                    TrabajoSO.Tamano = (int) ConvertirUnidadesAKBytes(Double.Parse(Console.ReadLine()));

                    if(TrabajoSO.Tamano == 0)
                    {
                        CambiarUnidades();
                        continue;
                    }

                    if (ConvertirKBytesAUnidades(TrabajoSO.Tamano) > maxSO) Advertencia("No puede ocupar más del 30% de la memoria total.");
                    if (TrabajoSO.Tamano < 0) Advertencia("No puede trabajar sin el Sistema Operativo .");

                } while (ConvertirKBytesAUnidades(TrabajoSO.Tamano) > maxSO || ConvertirKBytesAUnidades(TrabajoSO.Tamano) == 0);

                Particion ParticionSO = new Particion(TrabajoSO);
                Particiones.Add(ParticionSO); //automatica el SO se convierte en Particiones[0]

                MemoriaResN = (TamMemoriaN - ParticionSO.Tamano);
                
            }
            catch (Exception)
            {
                Advertencia("Ocurrió un error en la captura de datos.");

                TamMemoriaN = 0;
                Particiones = null;
                PreguntarTamaños();
            }

            ActualizarTabla();
        }

        private static void PreguntarParticiones()
        {
            Particion ParticionActual;

            while (true)
            {
                ActualizarTabla();

                try
                {
                    Console.WriteLine("Establezca el tamaño de la nueva partición. Memoria restante: " + ConvertirKBytesAUnidades(MemoriaResN) + " (" + txtUnidades + ").");
                    Nota("Introduzca un 0 para ver más opciones.");
                    double tamañoParticion = Double.Parse(Console.ReadLine());

                    if (tamañoParticion <= 0)
                    {
                        Console.WriteLine("Menú de opciones\n [1]-Cambiar unidades\n [2]-Finalizar captura.");
                        tamañoParticion = Int32.Parse(Console.ReadLine());

                        if (tamañoParticion == 1) CambiarUnidades();
                        else if (Particiones.Count == 1 || tamañoParticion < 0) throw new Exception();
                        if (tamañoParticion == 2) break;
                        continue;
                    }

                    if (ConvertirKBytesAUnidades(MemoriaResN)< tamañoParticion)
                    {
                        Advertencia("El tamaño de la partición excede la memoria disponible (" + ConvertirKBytesAUnidades(MemoriaResN) + " " + txtUnidades + ").");
                        continue;
                    }

                    MemoriaResN = MemoriaResN - ConvertirUnidadesAKBytes(tamañoParticion);
                    ParticionActual = new Particion((int) ConvertirUnidadesAKBytes(tamañoParticion));
                    Particiones.Add(ParticionActual);

                    if (MemoriaResN == 0)
                    {
                        ActualizarTabla();
                        Advertencia("Se ha agotado la memoria disponible.");
                        break;
                    }
                }
                catch (Exception)
                {
                    Advertencia("Ocurrió un problema en la captura de esta partición.");
                }
            }
        }

        private static void PreguntarTrabajos()
        {

            Boolean loop = true;
            while (loop)
            {
                ActualizarTabla();

                try
                {
                    Console.ForegroundColor = ConsoleColor.White;

                    Console.Write("Ingrese la cantidad de memoria que utilizará para el trabajo");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(" #" + (noTrabajoSiguiente));
                    Console.ForegroundColor = ConsoleColor.White;
                    Nota("Introduzca un 0 para ver más opciones.");
                    String txtIngresarTrabajo = Console.ReadLine();

                    if (txtIngresarTrabajo.Contains('#')){
                        int noTrabajo = Int32.Parse(txtIngresarTrabajo.Split('#')[1]);

                        foreach(Particion p in Particiones)
                        {
                            if (p.SO) continue;
                            if (p.Trabajo.NoTrabajo == noTrabajo)
                            {
                                BajaTrabajo(noTrabajo);
                                break;
                            }
                        }

                        if (StandBy == null) continue;
                        foreach(Trabajo t in StandBy)
                        {
                            if(t.NoTrabajo == noTrabajo)
                            {
                                StandBy.Remove(t);
                                ActualizarTabla();
                                Aviso("Se ha eliminado el trabajo #" + noTrabajo + " de la lista StandBy.");

                                Console.ReadKey();
                                break;
                            }
                        }

                        Aviso("El trabajo que busca no existe.");
                        continue;
                    }

                    double memoriaTrabajo = ConvertirUnidadesAKBytes(Double.Parse(txtIngresarTrabajo));
                    
                    if (memoriaTrabajo <= 0)
                    {
                        if(memoriaTrabajo == 0)
                        {
                            Console.WriteLine("Menú de opciones\n [1]-Cambiar unidades\n [2]-Continuar.");
                            memoriaTrabajo = Int32.Parse(Console.ReadLine());

                            if (memoriaTrabajo == 1) CambiarUnidades();
                            continue;
                        }

                        Aviso("Por favor ingrese una cantidad valida de memoria.");
                        Console.ReadKey();
                        continue;
                    }

                    AsignarTrabajo((int) memoriaTrabajo);

                }
                catch (Exception)
                {
                    Advertencia("Hubo un error al capturar este trabajo.");
                }
            }
        }

        private static void BajaTrabajo(int noTrabajo)
        {
            //Verificar que no se trate de una eliminación
            foreach (Particion p in Particiones)
            {
                if (p.SO) continue;
                if (p.Trabajo == null) continue;

                if (p.Trabajo.NoTrabajo == noTrabajo)
                {
                    Trabajo t = p.Trabajo;
                    p.Trabajo = null;

                    ActualizarTabla();
                    Aviso("El trabajo #" + t.NoTrabajo + " (" + ConvertirKBytesAUnidades(t.Tamano) + " " + txtUnidades + ") ha sido librado.");
                    Console.ReadKey();
                    t = null;

                    //Se liberó un espacio, ahora a comprobar si un trabajo en standby lo puede usar.
                    ActualizarStandBy();

                    return;
                }
            }
        }

        private static void AsignarTrabajo(int memoriaTrabajo)
        {

            Trabajo trabajo = new Trabajo((int) AsignacionNoTrabajo(), memoriaTrabajo);

            //Sacar la partición mayor
            int particionMayor = -1;
            foreach (Particion p in Particiones)
            {
                if (p.SO) continue;
                if (particionMayor < p.Tamano) particionMayor = p.Tamano;
            }
            if(trabajo.Tamano > particionMayor)
            {
                Aviso("El trabajo es demasiado grande y no podrá ser atendido.");
                Console.ReadKey();
                return;
            }

            //Alta
            foreach (Particion p in Particiones)
            {
                if (p.SO) continue;
                if (p.Trabajo != null) continue;
                if (p.Tamano < trabajo.Tamano) continue;
                
                p.Trabajo = trabajo;
                
                ActualizarTabla();
                Aviso("Se ha añadido el trabajo #" + trabajo.NoTrabajo + " (" + ConvertirKBytesAUnidades(memoriaTrabajo) + " " + txtUnidades + ").");
                Console.ReadKey();

                return;
            }

            //Si llega aquí significa que el trabajo estará en standby
            StandBy.Add(trabajo);

            Aviso("Se ha añadido el trabajo #" + trabajo.NoTrabajo + " (" + ConvertirKBytesAUnidades(memoriaTrabajo) + " " + txtUnidades + ") a la lista de StandBy.");
            Console.ReadKey();
        }

        private static void ActualizarStandBy()
        {
            //Este metodo intenta registrar todos los trabajos en StandBy posibles
            foreach(Trabajo t in StandBy) {

                foreach (Particion p in Particiones)
                {
                    if (p.SO) continue;
                    if (p.Trabajo != null) continue;
                    if (p.Tamano < t.Tamano) continue;
                    
                    p.Trabajo = t;

                    ActualizarTabla();
                    Aviso("Se ha añadido el trabajo #" + t.NoTrabajo + " (" + t.Tamano + " " + txtUnidades + ") de la lista en StandBy.");
                    StandBy.Remove(t);

                    Console.ReadKey();

                    return;
                }
            }
        }

        private static void MostrarStandBy()
        {
            int origY = Console.CursorTop, origX = Console.CursorLeft;

            int x = 80, y = 2;
            WriteAt("=====================", x, y++);
            WriteAt("Trabajos en StandBy", x, y++);
            WriteAt("=====================", x, y++);
            Console.ForegroundColor = ConsoleColor.Gray;
            WriteAt("Numero\tTamaño", x, y++);
            Console.ForegroundColor = ConsoleColor.Yellow;
            
            if(StandBy == null) WriteAt("No hay trabajos en StandBy", x, y++);
            else if (StandBy.Count < 1) WriteAt("No hay trabajos en StandBy", x, y++);
            else foreach (Trabajo t in StandBy) WriteAt("#"+t.NoTrabajo+"\t"+t.Tamano+" "+txtUnidades, x, y++);

            Console.ForegroundColor = ConsoleColor.White;
            
            WriteAt("", origX, origY);
        }

        private static double AsignacionNoTrabajo()
        {
            return noTrabajoSiguiente++;
        }

        private static void Advertencia(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("██████████████████████████████████████████████████████████████████████████████████████████");
            WriteAt("█████", 85, Console.CursorTop);
            Console.CursorLeft = 0;
            Console.WriteLine("█████ " + msg + " Presione cualquier tecla para continuar.");
            Console.WriteLine("██████████████████████████████████████████████████████████████████████████████████████████");
            Console.ForegroundColor = ConsoleColor.White;

            Console.ReadKey();
        }

        private static void Aviso(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("██████████████████████████████████████████████████████████████████████████████████████████");
            WriteAt("█████", 85, Console.CursorTop);
            Console.CursorLeft = 0;
            Console.WriteLine("█████ " + msg);
            Console.WriteLine("██████████████████████████████████████████████████████████████████████████████████████████");
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void Nota(string msg)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(msg);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void ActualizarTabla()
        {
            Console.Clear();

            /*Establece el numero de renglones que se usará*/
            double renglonesRestantes = ((TamMemoriaN == 0) ? null : new string[(int)(TamMemoriaN / divisor)]).Length;

            /*Dibuja la memoria vacía*/
            if (renglonesRestantes != 0 && Particiones.Count < 1)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("╔═══════════════════════════════════════╗");

                for (int i = 0; i < renglonesRestantes; i++)
                {
                    //Escribir en medio del espacio vacío
                    if (i == (int)(renglonesRestantes / 2))
                    {
                        Console.Write("║\tEspacio en Memoria: " + ConvertirKBytesAUnidades(TamMemoriaN) + " " + txtUnidades);
                        WriteAt("║", 40, Console.CursorTop);
                        Console.WriteLine();
                        continue;
                    }

                    Console.WriteLine("║\t\t\t\t\t║");
                }

                Console.WriteLine("╚═══════════════════════════════════════╝\n");
                Console.ForegroundColor = ConsoleColor.White;
                return;
            }

            int memoriaDesperdiciada = 0;
            int noParticion = 1;

            /*Relleno de particiones*/
            foreach (Particion p in Particiones)
            {
                if (p.SO)
                {
                    double renglonesSO = Particiones[0].Tamano / divisor;
                    renglonesRestantes -= renglonesSO;

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("╔═══════════════════════════════════════╗");

                    if (renglonesSO < 2) renglonesSO = 2;
                    for (int i = 1; i < renglonesSO; i++)
                    {
                        if (i == (int)(renglonesSO / 2))
                        {
                            Console.Write("║\tSistema Operativo (" 
                                +((unidades != 1) ? ConvertirKBytesAUnidades(Particiones[0].Tamano).ToString("N2") : Particiones[0].Tamano.ToString())
                                + " " + txtUnidades + ")");
                            WriteAt("║", 40, Console.CursorTop);
                            Console.WriteLine();
                            continue;
                        }

                        Console.WriteLine("║\t\t\t\t\t║");
                    }

                    Console.WriteLine("╠═══════════════════════════════════════╣");
                    Console.ForegroundColor = ConsoleColor.Green;

                    continue;
                }

                double renglonesParticion = p.Tamano / divisor;
                renglonesRestantes -= renglonesParticion;
                if (renglonesParticion < 2) renglonesParticion = 2;

                Console.ForegroundColor = (p.Trabajo == null) ? ConsoleColor.Green : ConsoleColor.Red;
                for (int i = 1; i < renglonesParticion; i++)
                {
                    if (i == (int)(renglonesParticion / 2))
                    {
                        if (p.Trabajo == null) Console.Write("║\tPartición disponible (" 
                            + ((unidades != 1) ? ConvertirKBytesAUnidades(p.Tamano).ToString("N2") : p.Tamano.ToString())
                            + " " + txtUnidades + ")");
                        if (p.Trabajo != null)
                        {
                            Console.Write("║\tTrabajo #" + p.Trabajo.NoTrabajo + " (" + ConvertirKBytesAUnidades(p.Trabajo.Tamano) + "/" + ConvertirKBytesAUnidades(p.Tamano) + " " + txtUnidades + ")\t\t\tDesperdicio: " + ConvertirKBytesAUnidades(p.Tamano - p.Trabajo.Tamano) + " " + txtUnidades);
                            memoriaDesperdiciada += (p.Tamano - p.Trabajo.Tamano);
                        }
                        WriteAt("║#"+noParticion, 40, Console.CursorTop);
                        Console.WriteLine();
                        continue;
                    }

                    Console.WriteLine("║\t\t\t\t\t║");
                }
                Console.WriteLine("╠═══════════════════════════════════════╣");
                Console.ForegroundColor = ConsoleColor.White;

                noParticion++;
            }
            
            //Rellenar el resto
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            for (int i = 0; i < renglonesRestantes-1; i++)
            {
                if (MemoriaResN == 0) break;

                //Escribir en medio del espacio vacío
                if (i == (int) (renglonesRestantes/ 2))
                {
                    Console.Write("║\tMemoria Libre: " + ConvertirKBytesAUnidades(MemoriaResN) + " " + txtUnidades);
                    WriteAt("║", 40, Console.CursorTop);
                    Console.WriteLine();
                    continue;
                }

                Console.WriteLine("║\t\t\t\t\t║");
            }
            Console.Write("╚═══════════════════════════════════════╝");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\tDesperdicio Total: " + ConvertirKBytesAUnidades(memoriaDesperdiciada) + " " + txtUnidades + "\n");
            Console.ForegroundColor = ConsoleColor.White;

            MostrarStandBy();
        }
        
        private static double ConvertirUnidadesAKBytes(double memoria)
        {
            for (int i = 1; i < unidades; i++)
                memoria *= 1024;

            return memoria;
        }

        private static double ConvertirKBytesAUnidades(double memoria)
        {
            for (int i = 1; i < unidades; i++)
                memoria /= 1024;

            return memoria;
        }

        private static void WriteAt(string s, int x, int y)
        {
            try
            {
                Console.SetCursorPosition(origCol + x, origRow + y);
                Console.Write(s);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Console.Clear();
                Console.WriteLine(e.Message);
            }
        }
    }
}
