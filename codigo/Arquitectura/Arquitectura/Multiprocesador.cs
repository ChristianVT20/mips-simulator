using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Collections;


namespace Arquitectura{
    class Multiprocesador{
        // constantes
        public const int CANTIDADINSTRUCCIONES = 75;

        // variables
        string casoP1, casoP2, casoP3; //Para contabilizar el reloj en cada procesador, se declara un string que indique en cuál caso ha caido la instrucción en curso. 
        int relojP1, relojP2, relojP3; // relojes para cada procesador
        int[] memoriaPrincipal;
        int[] memoriaInstrucciones;
        char[,] directorioP1;
        char[,] directorioP2;
        char[,] directorioP3;
        string[,] cacheP1;
        string[,] cacheP2;
        string[,] cacheP3;
        int contadorIGlobal, cantidadHilos, contadorResultados;
        Thread procesador1, procesador2, procesador3;
        static object bloqueador, bloqueadorC1, bloqueadorC2, bloqueadorC3, bloqueadorD1, bloqueadorD2, bloqueadorD3;
        static object bloqueadorT1, bloqueadorT2, bloqueadorT3, bloqueadorT4, bloqueadorI;
        private ArrayList listaRutas;
        private Resultados ventanaResultados; // ventana principal de los resultados
        private ResultadosPorHilo [] resultados; // ventanas de resultados para cada hilo


        public Multiprocesador(ArrayList _listaRutas){
            cantidadHilos = _listaRutas.Count;
            contadorIGlobal = 0; //Lleva la cuenta del hilo que sigue a procesar.
            contadorResultados = 0; // cuenta cual es el siguiente resultado a mostrar
            memoriaInstrucciones = new int[_listaRutas.Count * CANTIDADINSTRUCCIONES * 4];
            listaRutas = _listaRutas;
            // objetos para realizar los bloqueos
            bloqueadorT1 = new object();
            bloqueadorT2 = new object();
            bloqueadorT3 = new object();
            bloqueadorT4 = new object();
            bloqueadorI = new object();
            bloqueador = new object();
            bloqueadorC1 = new object();
            bloqueadorC2 = new object();
            bloqueadorC3 = new object();
            bloqueadorD1 = new object();
            bloqueadorD2 = new object();
            bloqueadorD3 = new object();
            memoriaPrincipal = new int[3 * 8 * 4];//Procesadores * Bloques * Palabras
            directorioP1 = new char[8, 4];
            directorioP2 = new char[8, 4];
            directorioP3 = new char[8, 4];
            cacheP1 = new string[6, 4];
            cacheP2 = new string[6, 4];
            cacheP3 = new string[6, 4];
            inicializarEstructuras();
            cargarHilos();
            procesador1 = new Thread(procesador);
            procesador2 = new Thread(procesador);
            procesador3 = new Thread(procesador);
            procesador1.Name = "Procesador1";
            procesador2.Name = "Procesador2";
            procesador3.Name = "Procesador3";
            casoP1 = "";
            casoP2 = "";
            casoP3 = "";
            relojP1 = 0;
            relojP2 = 0;
            relojP3 = 0;
            // se instancian las ventanas para mostrar los resultados
            ventanaResultados = new Resultados();
            resultados = new ResultadosPorHilo[cantidadHilos];
            for (int i = 0; i < cantidadHilos; i++) {
                resultados[i] = new ResultadosPorHilo();
                resultados[i].Show();
                resultados[i].Visible = false;
            }
        }

        /*
         * Inicia la ejecución de la simulación
         */
        public void ejecutar(){
            ventanaResultados.Show();
            procesador1.Start();
            procesador2.Start();
            procesador3.Start();
            procesador1.Join();
            procesador2.Join();
            procesador3.Join();
        }

        /*
         * Inicializa la memoria, las caches, y los directorios
         */
        private void inicializarEstructuras() {
            for (int i = 0; i < 8; i++){
                directorioP1[i, 0] = 'U';
                directorioP2[i, 0] = 'U';
                directorioP3[i, 0] = 'U';
                for (int k = 1; k <= 3; k++) {
                    directorioP1[i, k] = '0';
                    directorioP2[i, k] = '0';
                    directorioP3[i, k] = '0';
                }
            }

            // caches en 0
            for (int i = 0; i < 5; i++){
                for (int j = 0; j < 4; j++){
                    cacheP1[i, j] = "0";
                    cacheP2[i, j] = "0";
                    cacheP3[i, j] = "0";
                }
            }
            // estados Inválidos
            for (int j = 0; j < 4; j++)
            {
                cacheP1[5, j] = "I";
                cacheP2[5, j] = "I";
                cacheP3[5, j] = "I";
            }
            // memoria inicializada con su indice
            for (int i = 0; i < memoriaPrincipal.Length; i++ )
                memoriaPrincipal[i] = 0;           
        }

        /*
         *Carga las instrucciones de TODOS los hilos y los copia en una memoria de Instrucciones, 
         *la cual no pertenece al Procesador ni al Multiprocesador, sino, a los preparativos de la Simulación. 
         */
        private void cargarHilos(){
            int frontera; // punto donde inicia el programa "hilito"
            for (int i = 0; i < cantidadHilos; i++){
                frontera = i * (CANTIDADINSTRUCCIONES * 4);
                llenarMemInstrucciones(listaRutas[i].ToString(), frontera);
            }
        }

        /*
         * Aumenta el reloj para un procesador en cierta cantidad de ciclos
         */
        private void aumentarReloj(int ciclos, int procesador){
            switch(procesador){
                case 1:
                    relojP1 += ciclos;
                    ventanaResultados.ponerReloj(1, relojP1);
                    break;
                case 2:
                    relojP2 += ciclos;
                    ventanaResultados.ponerReloj(2, relojP2);
                    break;
                case 3:
                    relojP3 += ciclos;
                    ventanaResultados.ponerReloj(3, relojP3);
                    break;
           }
        }

        /*
         * Método principal de ejecución para cada procesador.
         * Este método simula la acción del procesador
         */
        public void procesador(){
            int[] registros = new int[32];
            int[] instrucciones;
            int PC; // puntero a la instruccion actual
            string nombre = Thread.CurrentThread.Name;
            int miIdProcesador = 0;
            // variables para el load y el store
            int posicionFisica;
            int posicionLogica;
            int bloque;
            int palabra;

           switch(nombre){ 
               case "Procesador1":
                   miIdProcesador = 1;
                   break;
               case "Procesador2":
                   miIdProcesador = 2;
                   break;
               case "Procesador3":
                   miIdProcesador = 3;
                   break;
           } 

            while (true){
                PC = 0;
                lock (bloqueador){ // el procesador solicita el programa ("hilito") siguiente para ejecutar
                    if (contadorIGlobal >= cantidadHilos){
                        break;
                    }
                    instrucciones = getInstrucciones();      
                }

                // ciclo de ejecución de instrucciones
                int codigoOp = instrucciones[PC];
                while (codigoOp != 63){
                    // decodificación de la instrucción     
                    int param1 = instrucciones[PC + 1];
                    int param2 = instrucciones[PC + 2];
                    int param3 = instrucciones[PC + 3];

                    // aumento del PC antes de ejecutar la instrucción leida
                    PC += 4;

                    switch (codigoOp){
                        case 8:  // DADDI
                            registros[param2] = registros[param1] + param3;
                            aumentarReloj(1, miIdProcesador); // aumenta el reloj
                            break;
                        case 32: // DADD
                            registros[param3] = registros[param1] + registros[param2];
                            aumentarReloj(1, miIdProcesador); // aumenta el reloj
                            break;
                        case 34: // DSUB
                            registros[param3] = registros[param1] - registros[param2];
                            aumentarReloj(1, miIdProcesador); // aumenta el reloj
                            break;
                        case 35: // LOAD
                            posicionFisica = registros[param1] + param3;
                            posicionLogica = posicionFisica / 4;
                            bloque = getBloque(posicionLogica);
                            palabra = posicionLogica % 4;
                            // realiza un load 
                            // palabra => numero de palabra que ocupa cargar del bloque
                            // registros[param2] => es registro al que se le quiere guardar la palabra
                            load(bloque, ref registros[param2], palabra, miIdProcesador);
                            contabilizarTiempo(miIdProcesador);
                            break;

                        case 43: // STORE
                            posicionFisica = registros[param1] + param3;
                            posicionLogica = posicionFisica / 4;
                            bloque = getBloque(posicionLogica);
                            palabra = posicionLogica % 4;
                            // realiza un store 
                            //palabra => numero de palabra que ocupa cargar del bloque
                            // registros[param2] => es registro que tiene el contenido a guardar en memoria
                            store(bloque, ref registros[param2], palabra, miIdProcesador);
                            contabilizarTiempo(miIdProcesador);
                            break;
                        case 4:  // BEQZ
                            if (registros[param1] == 0)
                                PC += param3 * 4;
                            aumentarReloj(1, miIdProcesador); // aumenta el reloj
                            break;
                        case 5: // BNEZ
                            if (registros[param1] != 0)
                                PC += param3 * 4;
                            aumentarReloj(1, miIdProcesador); // aumenta el reloj
                            break;
                        case 63: // fin
                            aumentarReloj(1, miIdProcesador); // aumenta el reloj
                            break;
                    }

                    // obtiene la siguiente instrucción a ejecutar
                    codigoOp = instrucciones[PC];
                } // fin de ejecución de instrucciones
                lock (bloqueador){  // muestra los resultados de la ejecución de cada "hilito"
                    mostrarMemoria();
                    mostrarRegistros(miIdProcesador, registros);
                    mostrarCaches();
                    mostrarDirectorios();
                    mostrarResultados();
                }
            }
        }

        // contabiliza el tiempo para un load o un store
        public void contabilizarTiempo(int procesador) {
            switch (procesador) { 
                case 1:
                    contabilizarTiempoPorCaso(casoP1, ref relojP1);
                    break;
                case 2:
                    contabilizarTiempoPorCaso(casoP2, ref relojP2);
                    break;
                case 3:
                    contabilizarTiempoPorCaso(casoP3, ref relojP3);
                    break;
            } 
        }

        // revisa el caso que se dió y aumenta el reloj correspondiente
        public void contabilizarTiempoPorCaso(string caso, ref int reloj) { 
                switch (caso){
                    case "IUoC": //Bloque víctima (I)nválido y estado del bloque que ocupa en estado (U o C)
                        reloj += 150;
                        break;
                    case "IM": //Bloque víctima (I)nválido y estado del bloque que ocupa en estado (M)
                        reloj += 250;
                        break;
                    case "CUoC": //Bloque víctima (C)ompartido y estado del bloque que ocupa en estado (U o C)
                        reloj += 200;
                        break;
                    case "CM": //Bloque víctima (C)ompartido y estado del bloque que ocupa en estado (M)
                        reloj += 300;
                        break;
                    case "MUoC": //Bloque víctima (M)odificado y estado del bloque que ocupa en estado (U o C)
                        reloj += 240;
                        break;
                    case "MM": //Bloque víctima (M)odificado y estado del bloque que ocupa en estado (M)
                        reloj += 340;
                        break;
                    case "HL": //Hit de cache en Load
                        reloj += 20;
                        break;
                    case "HCS": //Hit de cache en Store con bloque compartido
                        reloj += 100;
                        break;
                    case "HMS": //Hit de cache en Store con bloque modificado
                        reloj += 20;
                        break;
                    default:
                        //Console.WriteLine("Error contabilizarTiempoPorCaso..." + caso);
                        break;
                }  
        }

        // indica el estado en que cayo el load o el store
        public void setEstado(int miIdProcesador, string bloqueVictima, char estadoBloqueEnDir){
            lock (bloqueadorT2) {
                switch (miIdProcesador) { 
                case 1:
                    SetEstadoEspecifico(ref casoP1, bloqueVictima, estadoBloqueEnDir);
                    break;
                case 2:
                    SetEstadoEspecifico(ref casoP2, bloqueVictima, estadoBloqueEnDir);
                    break;
                case 3:
                    SetEstadoEspecifico(ref casoP3, bloqueVictima, estadoBloqueEnDir);
                    break;
                default:
                   //Console.WriteLine("Error en el método SetEstado...");
                    break;
                }
            }
            
        }

        // indica el estado en que cayo el load o el store
        public void SetEstadoEspecifico(ref string caso, string bloqueVictima, char estadoBloqueEnDir){
            lock(bloqueadorT3){
                switch (bloqueVictima){
                    case "I":
                        caso = "IUoC";//bloque víctima inválido y bloque en U o C.
                        if (estadoBloqueEnDir == 'M')
                            caso = "IM";//bloque víctima inválido y bloque en M.
                        break;
                    case "M":
                        caso = "MUoC";//bloque víctima modificado y bloque en U o C.
                        if (estadoBloqueEnDir == 'M')
                            caso = "MM";//bloque víctima modificado y bloque en M.
                        break;
                    case "C":
                        caso = "CUoC";//bloque víctima compartido y bloque en U o C.
                        if (estadoBloqueEnDir == 'M')
                            caso = "CM";//bloque víctima compartido y bloque en M.
                        //Console.WriteLine("Entro a: " + caso);
                        break;
                    case "HL"://Hit en Load.
                        caso = "HL";
                        break;
                    case "HCS"://Hit en store con estado Compartido.
                        caso = "HCS";
                        break;
                    case "HMS"://Hit en store con estado Modificado.
                        caso = "HMS";
                        break;
                    default:
                        //Console.WriteLine("Error en el método SetEstadoEspecifico...");
                        break;
                }
            } 
        }

/*--------------------------------------------------------------------------------------------------------------
        STORE
--------------------------------------------------------------------------------------------------------------*/

         /* hace el store
         */
        private void store(int bloque, ref int registro, int palabra, int miIdProcesador){
            bool resultado = false; // para indicar si el bloque estaba en su cache local
            string estadoMem = "I"; // estado del bloque en cache en donde va a caer el nuevo bloque
            int bloqueMem = -1; // numero de bloque que se encuentra compartido o modificado en la cache 
            string bloq = "" + bloque; //Se convierte a String. xq la cache esta en strings 
            char estadoBloqueEnDir = '\0'; //Indica cuál es el estado del bloque que necesito (para el reloj).
            string estadoBloqueEnDirHit = "";//Indica cuál es el estado del bloque que necesito (para el reloj).
            switch(miIdProcesador){ 
                case 1:
                    resultado = setDatoCache(ref cacheP1, ref bloqueadorC1, bloque, ref registro, palabra, ref estadoMem, ref bloqueMem, ref estadoBloqueEnDirHit);
                    break;
                case 2:
                    resultado = setDatoCache(ref cacheP2, ref bloqueadorC2, bloque, ref registro, palabra, ref estadoMem, ref bloqueMem, ref estadoBloqueEnDirHit);
                    break;
                case 3:
                    resultado = setDatoCache(ref cacheP3, ref bloqueadorC3, bloque, ref registro, palabra, ref estadoMem, ref bloqueMem, ref estadoBloqueEnDirHit);
                    break;
            }
            

            // Si no esta en cache
            if (resultado == false){
                // Si la posicion a donde va a caer esta modificada o compartida
                int adminBloqueModificado = getAdministrador(bloqueMem);
                if (estadoMem.Equals("M")) {
                    cambiarEstadoEnDirectorio(adminBloqueModificado, bloqueMem, miIdProcesador, 'M');
                }else if(estadoMem.Equals("C")){
                    cambiarEstadoEnDirectorio(adminBloqueModificado, bloqueMem, miIdProcesador, 'C');
                }

                int admin = getAdministrador(bloque);
                //El administrador del bloque es local (Es mi directorio el admin del bloque que voy a copiar a mi cache).
                // admin devuelve 0,1,2 y miIdProcesador es 1,2,3
                if (admin+1 == miIdProcesador){
                    cargarDeCacheStore('L', admin, bloque, miIdProcesador, ref registro, palabra, ref estadoBloqueEnDir);
                }else{ //El administrador del bloque es remoto
                    cargarDeCacheStore('R', admin, bloque, miIdProcesador, ref registro, palabra, ref estadoBloqueEnDir);
                }
            } // fin de if -> si no esta en cache
            if (resultado == true){ //Hit de caché
                if (estadoBloqueEnDirHit.Equals("C")){
                    setEstado(miIdProcesador, "HCS", estadoBloqueEnDir);
                }
                else {
                    if (estadoBloqueEnDirHit.Equals("M")) {
                        setEstado(miIdProcesador, "HMS", estadoBloqueEnDir);
                    }
                }
                
            }
            else{ //Fallo
                setEstado(miIdProcesador, estadoMem, estadoBloqueEnDir);
            }

        }

        // en este metodo se selecciona el directorio que administra el bloque que se necesita cargar.
        private void cargarDeCacheStore(char tipoDir, int admin, int bloque, int procesador, ref int registro, int palabra, ref char estadoBloqueEnDir){
            switch (admin){
                case 0:
                    cargarDeCacheEnDirectorioEspecificoStore(tipoDir, ref bloqueadorD1, ref directorioP1, bloque, procesador, ref registro, palabra, ref estadoBloqueEnDir);
                    break;
                case 1:
                    cargarDeCacheEnDirectorioEspecificoStore(tipoDir, ref bloqueadorD2, ref directorioP2, bloque, procesador, ref registro, palabra, ref estadoBloqueEnDir);
                    break;
                case 2:
                    cargarDeCacheEnDirectorioEspecificoStore(tipoDir, ref bloqueadorD3, ref directorioP3, bloque, procesador, ref registro, palabra, ref estadoBloqueEnDir);
                    break;
            } 
        }



        // Revisa el directorio para saber en que estado esta el bloque
        // además selecciona la cache del procesador donde esta corriendo para cargarle el bloque de memoria
        private void cargarDeCacheEnDirectorioEspecificoStore(char tipoDir, ref object bloqueadorDir, ref char[,] directorio, int bloque, int procesador, ref int registro, int palabra, ref char estadoBloqueEnDir)
        {
            switch (procesador){
                case 1:
                    cargarDeCacheEnDirectorioEspecificoCacheLocalStore(tipoDir, ref bloqueadorDir, ref directorio, ref bloqueadorC1, ref cacheP1, bloque, procesador, ref registro, palabra,ref estadoBloqueEnDir);
                    break;
                case 2:
                    cargarDeCacheEnDirectorioEspecificoCacheLocalStore(tipoDir, ref bloqueadorDir, ref directorio, ref bloqueadorC2, ref cacheP2, bloque, procesador, ref registro, palabra, ref estadoBloqueEnDir);
                    break;
                case 3:
                    cargarDeCacheEnDirectorioEspecificoCacheLocalStore(tipoDir, ref bloqueadorDir, ref directorio, ref bloqueadorC3, ref cacheP3, bloque, procesador, ref registro, palabra, ref estadoBloqueEnDir);
                    break;
            } 
        }
        
        // recibe el directorio a revisar y mi cache
        // revisa si el bloque que debe traer a cache no esta en el directorio como 'U', 'M' o 'C' y realiza las acciones correspondientes 
        // si no logra obtener los 2 bloqueos los libera
        // tipo dir indica si el directorio es local o remoto
        private void cargarDeCacheEnDirectorioEspecificoCacheLocalStore(char tipoDir, ref object bloqueadorDir, ref char[,] directorio, ref object bloqueadorMC, ref string[,] cache, int bloque, int procesador, ref int registro, int palabra, ref char estadoBloqueEnDir){ 
            bool bloqueoAdquirido = false;
            int posicion = bloque % 8;
            int contador = 0; // futuro reloj
            char estadoBloque = '\0';
            while (!bloqueoAdquirido) { // ESPERA ACTIVA  
                try{
                    Monitor.TryEnter(bloqueadorDir, ref bloqueoAdquirido);
                    if (bloqueoAdquirido){
                        estadoBloque = directorio[posicion, 0];
                        estadoBloqueEnDir = estadoBloque; //Estado del bloque que se usa, para contabilizar el tiempo
                        string etiquetaNueva = ""+bloque;
                        // si el estado del bloque es 'U' lo copia a la cache, modifica el directorio y escribe
                        if (estadoBloque == 'U') { 
                            // trata de bloquear su cache
                            if (Monitor.TryEnter(bloqueadorMC)){//
                                try{
                                    // trae el bloque a su cache
                                    int posIni = bloque * 4;
                                    for (int i = 0; i < 4; i++)
                                       cache[i, bloque % 4] =  ""+memoriaPrincipal[i+posIni];
                                    // cambia la cache y el directorio
                                    cache[4, bloque % 4] = etiquetaNueva;
                                    cache[5, bloque % 4] = "M";
                                    directorio[bloque % 8, 0] = 'M';
                                    directorio[bloque % 8, procesador] = '1'; 
                                    // guarda en el registro la palabra solicitada 
                                    cache[palabra, bloque % 4] = ""+registro;
                                }finally{
                                    Monitor.Exit(bloqueadorMC);
                                }
                            }else{ // si no logra bloquear el segundo recurso, libera el primero.
                                contador++;
                                bloqueoAdquirido = false;
                                Monitor.Exit(bloqueadorDir);
                            }
                        }

                        // si el bloque que va a traer esta compartido
                        // invalida las otras caches q lo tienen compartido y modifica el directorio
                        // luego lo trae de memoria a menos que lo tenga en cache compartido solo por el
                        // y finalmente escribe
                        if(estadoBloque == 'C'){ 
                            invalidarBloqueCompartidoEnCaches(tipoDir, ref bloqueadorDir, ref directorio, ref bloqueadorMC, ref cache,  bloque, procesador, ref registro, palabra, ref bloqueoAdquirido, ref contador);
                            cache[palabra, bloque % 4] = "" + registro;
                        }

                        // Si el estado es 'M'
                        // cambia el estado del bloque que lo tiene modificado, además del directorio
                        // luego lo copia a su caché y finalmente lo escribe
                        if(estadoBloque == 'M'){ 
                            // bloquear la cache que lo tiene modificado
                            int numCache = 0;
                            for (int i = 1; i < 4; i++)
                                if (directorio[posicion, i] == '1')
                                    numCache = i; 
                            switch(numCache){
                                case 1:
                                    if (Monitor.TryEnter(bloqueadorC1)){
                                        try{
                                            copiarDeCacheAMemoriaStore(ref cacheP1, ref directorio, bloque);
                                        }finally{
                                            Monitor.Exit(bloqueadorC1);
                                            //Suelta la caché remota y agarra la caché local 
                                            if (Monitor.TryEnter(bloqueadorMC)){//
                                                try{
                                                    copiarDeMemoriaACacheStore(ref cache, ref directorio, bloque, procesador);
                                                    // guarda en la cache respectiva el registro que deseamos guardar 
                                                    cache[palabra, bloque % 4] = "" + registro;
                                                }finally{
                                                    Monitor.Exit(bloqueadorMC);
                                                }
                                            }else{ // si no logra bloquear el segundo recurso, libera el primero.
                                                contador++;
                                                bloqueoAdquirido = false;
                                                Monitor.Exit(bloqueadorDir);
                                            }
                                        }
                                    }else{ // si no logra bloquear el segundo recurso, libera el primero.
                                        contador++;
                                        bloqueoAdquirido = false;
                                        Monitor.Exit(bloqueadorDir);
                                    }
                                    break;

                                case 2:
                                    if (Monitor.TryEnter(bloqueadorC2)){//
                                        try{
                                            copiarDeCacheAMemoriaStore(ref cache, ref directorio, bloque);
                                        }finally{
                                            //Suelta la caché remota y agarra la caché local 
                                            Monitor.Exit(bloqueadorC2);
                                            
                                            if (Monitor.TryEnter(bloqueadorMC)){//
                                                try{
                                                    copiarDeMemoriaACacheStore(ref cacheP2, ref directorio, bloque, procesador);
                                                    // guarda en la cache respectiva el registro que deseamos guardar 
                                                    cache[palabra, bloque % 4] = "" + registro;
                                                }finally{
                                                    Monitor.Exit(bloqueadorMC);
                                                }
                                            }else{ // si no logra bloquear el segundo recurso, libera el primero.
                                                contador++;
                                                bloqueoAdquirido = false;
                                                Monitor.Exit(bloqueadorDir);
                                            }
                                            
                                        }
                                    }else{ // si no logra bloquear el segundo recurso, libera el primero.
                                        contador++;
                                        bloqueoAdquirido = false;
                                        Monitor.Exit(bloqueadorDir);
                                    }
                                    break;

                                case 3:
                                    if (Monitor.TryEnter(bloqueadorC3)){//
                                        try{
                                            copiarDeCacheAMemoriaStore(ref cacheP3, ref directorio, bloque);
                                        }finally{
                                            Monitor.Exit(bloqueadorC3);
                                            if (Monitor.TryEnter(bloqueadorMC)){//
                                                try{
                                                    copiarDeMemoriaACacheStore(ref cache, ref directorio, bloque, procesador);
                                                    // guarda en la cache respectiva el registro que deseamos guardar 
                                                    cache[palabra, bloque % 4] = "" + registro;
                                                }finally{
                                                    Monitor.Exit(bloqueadorMC);
                                                }
                                            }else{ // si no logra bloquear el segundo recurso, libera el primero.
                                                contador++;
                                                bloqueoAdquirido = false;
                                                Monitor.Exit(bloqueadorDir);
                                            }
                                        }
                                    }else{ // si no logra bloquear el segundo recurso, libera el primero.
                                        contador++;
                                        bloqueoAdquirido = false;
                                        Monitor.Exit(bloqueadorDir);
                                    }
                                    break;
                            }
                        }

                    }else
                        contador++; // aqui aumenta el reloj
                }finally{
                    if (bloqueoAdquirido){
                        Monitor.Exit(bloqueadorDir);
                    }
                }
            } // fin de while 
        }

        // invalida los bloques que tienen compartido un bloque que va a ser modificado
        // en este momento tiene el directorio bloqueado, asi que se van a ir obteniendo un bloqueo a la vez x cada cache q lo tenga compartido
        // para invalidarla, en caso de que no pueda obtener algun bloqueo se libera el directorio, y se mantiene la coherencia de cachés
        // si no la tiene compartido con el mismo, al final del método se copia el bloque a su caché
        private void invalidarBloqueCompartidoEnCaches(char tipoDir, ref object bloqueadorDir, ref char[,] directorio, ref object bloqueadorMC, ref string[,] cache, int bloque, int procesador, ref int registro, int palabra, ref bool bloqueoAdquirido, ref int contador){
            int posicion = bloque % 8;
            if (directorio[posicion, 1] == '1') { //compartido con cache 1
                if (Monitor.TryEnter(bloqueadorC1)){//
                    try{
                        // aqui sigue
                        directorio[posicion, 0] = 'U';
                        directorio[posicion, 1] = '0';
                        cacheP1[5, bloque%4] = "I";
                    }finally{
                        Monitor.Exit(bloqueadorC1);
                    }
                }else{ // si no logra agarrar la cache1 suelta el directorio.
                    contador++;
                    bloqueoAdquirido = false;
                    Monitor.Exit(bloqueadorDir);
                }
            }
            if (directorio[posicion, 2] == '1' && bloqueoAdquirido) {//compartido con cache 2
                if (Monitor.TryEnter(bloqueadorC2)){//
                    try{
                        directorio[posicion, 0] = 'U';
                        directorio[posicion, 2] = '0';
                        cacheP2[5, bloque % 4] = "I";
                    }finally{
                        Monitor.Exit(bloqueadorC2); 
                    }
                }else{ // si no logra agarrar la cache2 suelta el directorio y deja todo consistente
                    contador++;
                    directorio[posicion, 0] = 'C'; // para consistencia caches-directorio
                    bloqueoAdquirido = false;
                    Monitor.Exit(bloqueadorDir);
                }
            }
            if (directorio[posicion, 3] == '1' && bloqueoAdquirido) {//compartido con cache 3
                if (Monitor.TryEnter(bloqueadorC3)){//
                    try{
                        directorio[posicion, 0] = 'U';
                        directorio[posicion, 3] = '0';
                        cacheP3[5, bloque % 4] = "I";
                    }finally{
                        Monitor.Exit(bloqueadorC3);
                    }
                }else{ // si no logra agarrar la cache3 suelta el directorio y deja todo consistente
                    contador++;
                    directorio[posicion, 0] = 'C';
                    bloqueoAdquirido = false;
                    Monitor.Exit(bloqueadorDir);
                }
            }
            // ya revise las 3 caches e hice los cambios
            // ahora escribo a mi cache el bloque 
            if (Monitor.TryEnter(bloqueadorMC) && bloqueoAdquirido){//
                try{
                    copiarDeMemoriaACacheStore(ref cache, ref directorio, bloque, procesador); 
                }finally{
                    Monitor.Exit(bloqueadorMC); 
                }
            }else{ // si no logra agarrar la cache2 suelta el directorio y deja todo consistente
                contador++;
                bloqueoAdquirido = false;
                Monitor.Exit(bloqueadorDir);
            }


        }

        //Copia un bloque de la caché a memoria.
        private void copiarDeCacheAMemoriaStore(ref string[,] cache, ref char[,] directorio, int bloque){
            int posIni = bloque * 4;
            for (int i = 0; i < 4; i++)
                memoriaPrincipal[i + posIni] =  Convert.ToInt32(cache[i, bloque % 4]); //Copia el bloque de Caché a Memoria
            // cambia la cache y en el directorio pongo en la posicion correspondiente un 0
            cache[5, bloque % 4] = "I";
            directorio[bloque % 8, 0] = 'U';
            directorio[bloque % 8, 1] = '0';
            directorio[bloque % 8, 2] = '0';
            directorio[bloque % 8, 3] = '0';
        }

        //Copia un bloque de la memoria a caché.
        private void copiarDeMemoriaACacheStore(ref string[,] cache, ref char[,] directorio, int bloque, int procesador) {
            string etiquetaNueva = "" + bloque;
            int posIni = bloque * 4;
            for (int i = 0; i < 4; i++)
                cache[i, bloque % 4] = "" + memoriaPrincipal[i + posIni];
            // cambia la cache y el directorio
            cache[4, bloque % 4] = etiquetaNueva;
            cache[5, bloque % 4] = "M";
            directorio[bloque % 8, 0] = 'M';
            directorio[bloque % 8, procesador] = '1';
        }

       

        /* aqui revisa la cache y si esta agarra escribe el dato 
         * si no esta verifica, si la posicion donde va a caer esta modificada o compartida   
         * Recibe: cache que va a leer, el bloqueador de esa cache, estadoMem => estado en la cache de la posicion a donde va a caer, bloqueMem => el bloque que esta en la posicion donde va a caer
         */
        private bool setDatoCache(ref string[,] cache, ref object bloqueador, int bloque, ref int registro, int palabra, ref string estadoMem, ref int bloqueMem, ref string estadoBloqueEnDirHit){
            int contador = 0; // futuro reloj, debe recibirse por referencia
            bool resultado = false;
            bool bloqueoAdquirido = false;
            String bloq = "" + bloque;
            int bloqueCache = bloque % 4;
            string etiqueta;
            string estado = "";
            while (!bloqueoAdquirido){ // ESPERA ACTIVA  
                try{
                    Monitor.TryEnter(bloqueador, ref bloqueoAdquirido);
                    if (bloqueoAdquirido){
                        for (int i = 0; i < 4; i++){
                            etiqueta = cache[4, i]; // Número de bloque.
                            estado = cache[5, i];
                            estadoBloqueEnDirHit = estado;
                            // Si el bloque esta en cache y se encuentra modificado lo escribe en cache
                            if (etiqueta.Equals(bloq) && estado.Equals("M") ){ 
                                cache[palabra, bloqueCache] = ""+registro;
                                resultado = true;
                            }
                        }

                        if (resultado == false) { // este es el caso donde no esta en cache y el bloque donde va a caer esta modificado o compartido
                            estado = cache[5, bloqueCache];
                            etiqueta = cache[4, bloqueCache]; // numero de bloque 
                            if (estado.Equals("M") || estado.Equals("C")) {
                                estadoMem = estado;
                                bloqueMem = Convert.ToInt32(etiqueta);
                            }
                        }
                    }else
                        contador++; // aqui aumenta el reloj
                }finally{
                    if (bloqueoAdquirido){
                        Monitor.Exit(bloqueador);
                    }
                }
            }
            return resultado;
        }

/*--------------------------------------------------------------------------------------------------------------
        LOAD
--------------------------------------------------------------------------------------------------------------*/

        /* hace un load
         * Recibe el numero de bloque en memoria a cargar, el registro donde se va a guardar la palabra, la palabra del bloque y el id del procesador que realiza la operación
         */
        private void load(int bloque, ref int registro, int palabra, int miIdProcesador){
            bool resultado = false; // para indicar si el bloque estaba en su cache local
            string estadoMem = "I"; // estado del bloque en cache en donde va a caer el nuevo bloque
            int bloqueMem = -1; // numero de bloque que se encuentra compartido o modificado en la cache 
            string bloq = "" + bloque; //Se convierte a String. xq la cache esta en strings 
            char estadoBloqueEnDir = '\0'; //Indica cuál es el estado del bloque que necesito (para el reloj). 
            switch(miIdProcesador){
                case 1:
                    resultado = getDatoCache(ref cacheP1, ref bloqueadorC1, bloque, ref registro, palabra, ref estadoMem, ref bloqueMem);
                    break;
                case 2:
                    resultado = getDatoCache(ref cacheP2, ref bloqueadorC2, bloque, ref registro, palabra, ref estadoMem, ref bloqueMem);
                    break;
                case 3:
                    resultado = getDatoCache(ref cacheP3, ref bloqueadorC3, bloque, ref registro, palabra, ref estadoMem, ref bloqueMem);
                    break;
            }
            

            
            if (resultado == false){ // Si no esta en cache
                // Si la posicion a donde va a caer esta modificada o compartida
                int adminBloqueModificado = getAdministrador(bloqueMem);
                if (estadoMem.Equals("M")) {
                    cambiarEstadoEnDirectorio(adminBloqueModificado, bloqueMem, miIdProcesador, 'M');
                }else if(estadoMem.Equals("C")){
                    cambiarEstadoEnDirectorio(adminBloqueModificado, bloqueMem, miIdProcesador, 'C');
                }

                
                int admin = getAdministrador(bloque);
                //El administrador del bloque es local (Es mi directorio el admin del bloque que voy a copiar a mi cache).
                // admin devuelve 0,1,2 y miIdProcesador es 1,2,3
                if (admin+1 == miIdProcesador){
                    cargarDeCache('L', admin, bloque, miIdProcesador, ref registro, palabra, ref estadoBloqueEnDir);
                }else{ //El administrador del bloque es remoto
                    cargarDeCache('R', admin, bloque, miIdProcesador, ref registro, palabra, ref estadoBloqueEnDir);
                }
            } // fin de if -> si no esta en cache

            //Se guarda el estado para contabilizar su tiempo. ___________________________________________________________________________
            
            if(resultado == true){ //Hit de caché
                setEstado(miIdProcesador, "HL", estadoBloqueEnDir);
            }else{ //Fallo
                //Console.WriteLine("Estado mem 1 p: " + estadoMem + " EstadobloqDir p2: " + estadoBloqueEnDir);
                setEstado(miIdProcesador, estadoMem, estadoBloqueEnDir);
            }
            //____________________________________________________________________________________________________________________________
            

        }

        // carga un bloque que esta en otra cache a la cache del procesador que esta pidiendo el bloque
        private void cargarDeCache(char tipoDir, int admin, int bloque, int procesador, ref int registro, int palabra, ref char estadoBloqueEnDir){
            switch (admin){
                case 0:
                    cargarDeCacheEnDirectorioEspecifico(tipoDir, ref bloqueadorD1, ref directorioP1, bloque, procesador, ref registro, palabra, ref estadoBloqueEnDir);
                    break;
                case 1:
                    cargarDeCacheEnDirectorioEspecifico(tipoDir, ref bloqueadorD2, ref directorioP2, bloque, procesador, ref registro, palabra, ref estadoBloqueEnDir);
                    break;
                case 2:
                    cargarDeCacheEnDirectorioEspecifico(tipoDir, ref bloqueadorD3, ref directorioP3, bloque, procesador, ref registro, palabra, ref estadoBloqueEnDir);
                    break;
            } 
        }



        // Revisa el directorio para saber en que estado esta el bloque
        private void cargarDeCacheEnDirectorioEspecifico(char tipoDir, ref object bloqueadorDir, ref char[,] directorio, int bloque, int procesador, ref int registro, int palabra, ref char estadoBloqueEnDir)
        {
            switch (procesador){
                case 1:
                    cargarDeCacheEnDirectorioEspecificoCacheLocal(tipoDir, ref bloqueadorDir, ref directorio, ref bloqueadorC1, ref cacheP1, bloque, procesador, ref registro, palabra, ref estadoBloqueEnDir);
                    break;
                case 2:
                    cargarDeCacheEnDirectorioEspecificoCacheLocal(tipoDir, ref bloqueadorDir, ref directorio, ref bloqueadorC2, ref cacheP2, bloque, procesador, ref registro, palabra, ref estadoBloqueEnDir);
                    break;
                case 3:
                    cargarDeCacheEnDirectorioEspecificoCacheLocal(tipoDir, ref bloqueadorDir, ref directorio, ref bloqueadorC3, ref cacheP3, bloque, procesador, ref registro, palabra, ref estadoBloqueEnDir);
                    break;
            } 
        }
        
        // recibe el directorio a revisar y mi cache
        // tipo dir indica si el administrador del bloque es un directorio local o remoto
        private void cargarDeCacheEnDirectorioEspecificoCacheLocal(char tipoDir, ref object bloqueadorDir, ref char[,] directorio, ref object bloqueadorMC, ref string[,] cache, int bloque, int procesador, ref int registro, int palabra, ref char estadoBloqueEnDir) { 
            bool bloqueoAdquirido = false;
            int posicion = bloque % 8;
            int contador = 0; // futuro reloj
            char estadoBloque = '\0';
            while (!bloqueoAdquirido) { // ESPERA ACTIVA  
                try{
                    Monitor.TryEnter(bloqueadorDir, ref bloqueoAdquirido);
                    if (bloqueoAdquirido){
                        estadoBloque = directorio[posicion, 0];
                        estadoBloqueEnDir = estadoBloque;//Para registrar el estado del bloque a la hora de contabilizar los tiempos
                        string etiquetaNueva = ""+bloque;
                        if (estadoBloque == 'U' || estadoBloque == 'C') { 
                            // trata de bloquear su cache
                            if (Monitor.TryEnter(bloqueadorMC)){//
                                try{
                                    // trae el bloque a su cache
                                    int posIni = bloque * 4;
                                    for (int i = 0; i < 4; i++)
                                       cache[i, bloque % 4] =  ""+memoriaPrincipal[i+posIni];
                                    // cambia la cache y el directorio
                                    cache[4, bloque % 4] = etiquetaNueva;
                                    cache[5, bloque % 4] = "C";
                                    directorio[bloque % 8, 0] = 'C';
                                    directorio[bloque % 8, procesador] = '1'; 
                                    // guarda en el registro la palabra solicitada 
                                    registro = Convert.ToInt32(cache[palabra, bloque % 4]);
                                }finally{
                                    Monitor.Exit(bloqueadorMC);
                                }
                            }else{ // si no logra bloquear el segundo recurso, libera el primero.
                                contador++;
                                bloqueoAdquirido = false;
                                Monitor.Exit(bloqueadorDir);
                            }
                        }else{ // Si el estado es 'M'
                            // bloquear la cache que lo tiene modificado
                            int numCache = 0;
                            for (int i = 1; i < 4; i++)
                                if (directorio[posicion, i] == '1')
                                    numCache = i; 
                            switch(numCache){
                                case 1:
                                    if (Monitor.TryEnter(bloqueadorC1)){//
                                        try{
                                            copiarDeCacheAMemoria(ref cacheP1, ref directorio, bloque);
                                        }finally{
                                            Monitor.Exit(bloqueadorC1);
                                            //Suelta la caché remota y agarra la caché local 
                                            if (Monitor.TryEnter(bloqueadorMC)){//
                                                try{
                                                    copiarDeMemoriaACache(ref cache, ref directorio, bloque, procesador);
                                                    // guarda en el registro la palabra solicitada 
                                                    registro = Convert.ToInt32(cache[palabra, bloque % 4]);
                                                }finally{
                                                    Monitor.Exit(bloqueadorMC);
                                                }
                                            }else{ // si no logra bloquear el segundo recurso, libera el primero.
                                                contador++;
                                                bloqueoAdquirido = false;
                                                Monitor.Exit(bloqueadorDir);
                                            }
                                        }
                                    }else{ // si no logra bloquear el segundo recurso, libera el primero.
                                        contador++;
                                        bloqueoAdquirido = false;
                                        Monitor.Exit(bloqueadorDir);
                                    }
                                    break;

                                case 2:
                                    if (Monitor.TryEnter(bloqueadorC2)){//
                                        try{
                                            copiarDeCacheAMemoria(ref cache, ref directorio, bloque);
                                        }finally{
                                            //Suelta la caché remota y agarra la caché local 
                                            Monitor.Exit(bloqueadorC2);
                                            
                                            if (Monitor.TryEnter(bloqueadorMC)){//
                                                try{
                                                    copiarDeMemoriaACache(ref cacheP2, ref directorio, bloque, procesador);
                                                    // guarda en el registro la palabra solicitada 
                                                    registro = Convert.ToInt32(cache[palabra, bloque % 4]);
                                                }finally{
                                                    Monitor.Exit(bloqueadorMC);
                                                }
                                            }else{ // si no logra bloquear el segundo recurso, libera el primero.
                                                contador++;
                                                bloqueoAdquirido = false;
                                                Monitor.Exit(bloqueadorDir);
                                            }
                                            
                                        }
                                    }else{ // si no logra bloquear el segundo recurso, libera el primero.
                                        contador++;
                                        bloqueoAdquirido = false;
                                        Monitor.Exit(bloqueadorDir);
                                    }
                                    break;

                                case 3:
                                    if (Monitor.TryEnter(bloqueadorC3)){//
                                        try{
                                            copiarDeCacheAMemoria(ref cacheP3, ref directorio, bloque);
                                        }finally{
                                            Monitor.Exit(bloqueadorC3);
                                            if (Monitor.TryEnter(bloqueadorMC)){//
                                                try{
                                                    copiarDeMemoriaACache(ref cache, ref directorio, bloque, procesador);
                                                    // guarda en el registro la palabra solicitada 
                                                    registro = Convert.ToInt32(cache[palabra, bloque % 4]);
                                                }finally{
                                                    Monitor.Exit(bloqueadorMC);
                                                }
                                            }else{ // si no logra bloquear el segundo recurso, libera el primero.
                                                contador++;
                                                bloqueoAdquirido = false;
                                                Monitor.Exit(bloqueadorDir);
                                            }
                                        }
                                    }else{ // si no logra bloquear el segundo recurso, libera el primero.
                                        contador++;
                                        bloqueoAdquirido = false;
                                        Monitor.Exit(bloqueadorDir);
                                    }
                                    break;
                            }
                        }

                    }else
                        contador++; // aqui aumenta el reloj
                }finally{
                    if (bloqueoAdquirido){
                        Monitor.Exit(bloqueadorDir);
                    }
                }
            } // fin de while 
        }

        //Copia un bloque de la caché a memoria.
        private void copiarDeCacheAMemoria(ref string[,] cache, ref char[,] directorio, int bloque){
            int posIni = bloque * 4;
            for (int i = 0; i < 4; i++)
                memoriaPrincipal[i + posIni] =  Convert.ToInt32(cache[i, bloque % 4]); //Copia el bloque de Caché a Memoria
            // cambia la cache y el directorio
            cache[5, bloque % 4] = "C";
            directorio[bloque % 8, 0] = 'C';
        }

        //Copia un bloque de la memoria a caché.
        private void copiarDeMemoriaACache(ref string[,] cache, ref char[,] directorio, int bloque, int procesador) {
            string etiquetaNueva = "" + bloque;
            int posIni = bloque * 4;
            for (int i = 0; i < 4; i++)
                cache[i, bloque % 4] = "" + memoriaPrincipal[i + posIni];
            // cambia la cache y el directorio
            cache[4, bloque % 4] = etiquetaNueva;
            cache[5, bloque % 4] = "C";
            directorio[bloque % 8, 0] = 'C';
            directorio[bloque % 8, procesador] = '1';
        }

        // este metodo es lo primero que ejecuta un load
        // obtiene el bloqueo de su cache y verifica si tiene el bloque en su cache en estado modificado y compartido
        // de ser asi, lee directamente de su cache y no verifica los directorios.
        private bool getDatoCache(ref string[,] cache, ref object bloqueador, int bloque, ref int registro, int palabra, ref string estadoMem, ref int bloqueMem) {
            int contador = 0; // futuro reloj, debe recibirse por referencia
            bool resultado = false;
            bool bloqueoAdquirido = false;
            String bloq = "" + bloque;
            int bloqueCache = bloque % 4;
            string etiqueta;
            string estado = "";
            while (!bloqueoAdquirido){ // ESPERA ACTIVA  
                try{
                    Monitor.TryEnter(bloqueador, ref bloqueoAdquirido);
                    if (bloqueoAdquirido){
                        for (int i = 0; i < 4; i++){
                            etiqueta = cache[4, i]; //Número de bloque.
                            estado = cache[5, i];
                            //Si el bloque esta en cache y se encuentra compartido o modificado
                            if ((etiqueta.Equals(bloq)) && ((estado.Equals("M")) || (estado.Equals("C")))){ 
                                registro = Convert.ToInt32(cache[palabra, bloqueCache]);
                                resultado = true;
                            }
                        }

                        if (resultado == false) { // este es el caso donde no esta en cache y el bloque donde va a caer esta modificado
                            estado = cache[5, bloqueCache];
                            etiqueta = cache[4, bloqueCache]; // numero de bloque 
                            if (estado.Equals("M") || estado.Equals("C")) {
                                estadoMem = estado;
                                bloqueMem = Convert.ToInt32(etiqueta);
                            }
                        }
                    }else
                        contador++; // aqui aumenta el reloj
                }finally{
                    if (bloqueoAdquirido){
                        Monitor.Exit(bloqueador);
                    }
                }
            }
            return resultado;
        }

/*---------------------------------------------------------------------------------------------------------------------------------------------------------------
*              METODOS USADOS POR EL LOAD Y EL STORE
------------------------------------------------------------------------------------------------------------------------------------------------------------*/
        // elimina el estado compartido en el directorio y en la cache
        private void cambiarEstadoEnDirectorio(int admin, int bloque, int procesador, char estadoACambiar){
            switch (admin){
                case 0:
                    cambiarEstadoEnDirectorioEspecifico(ref bloqueadorD1, ref directorioP1, bloque, procesador, estadoACambiar);
                    break;
                case 1:
                    cambiarEstadoEnDirectorioEspecifico(ref bloqueadorD2, ref directorioP2, bloque, procesador, estadoACambiar);
                    break;
                case 2:
                    cambiarEstadoEnDirectorioEspecifico(ref bloqueadorD3, ref directorioP3, bloque, procesador, estadoACambiar);
                    break;
            } 
        }

        // elimina el estado compartido en el directorio y en la cache
        private void cambiarEstadoEnDirectorioEspecifico(ref object bloqueadorDir, ref char[,] directorio, int bloque, int procesador, char estadoACambiar){
            switch (procesador){
                case 1:
                    cambiarEstadoEnDirectorioEspecificoCacheEspecifica(ref bloqueadorDir, ref bloqueadorC1, ref directorio, ref cacheP1, bloque, procesador, estadoACambiar);
                    break;
                case 2:
                    cambiarEstadoEnDirectorioEspecificoCacheEspecifica(ref bloqueadorDir, ref bloqueadorC2, ref directorio, ref cacheP2, bloque, procesador, estadoACambiar);
                    break;
                case 3:
                    cambiarEstadoEnDirectorioEspecificoCacheEspecifica(ref bloqueadorDir, ref bloqueadorC3, ref directorio, ref cacheP3, bloque, procesador, estadoACambiar);
                    break;
            } 
        }

        private void cambiarEstadoEnDirectorioEspecificoCacheEspecifica(ref object bloqueadorDir, ref object bloqueadorC, ref char[,] directorio, ref string[,] cache, int bloque, int procesador, char estadoACambiar){
            int posicion = bloque%8;
            bool bloqueoAdquirido = false;
            int contador = 0; // futuro reloj
            while (!bloqueoAdquirido) { // ESPERA ACTIVA  
                try{
                    Monitor.TryEnter(bloqueadorDir, ref bloqueoAdquirido);
                    if (bloqueoAdquirido){
                        if (Monitor.TryEnter(bloqueadorC)){//aca es la parte donde lee las caches
                            try{
                                // suma es para saber 
                                int suma = (int) Char.GetNumericValue(directorio[posicion, 1]) + (int) Char.GetNumericValue(directorio[posicion, 2]) + (int) Char.GetNumericValue(directorio[posicion, 3]);
                                // si esta compartido
                                directorio[posicion, procesador] = '0';
                                cache[5, posicion % 4] = "I";
                                if (suma == 1){// si solo esta compartido por el mismo o modificado
                                    directorio[posicion, 0] = 'U';
                                    // si estaba modificado el bloque donde va a caer
                                    if (estadoACambiar == 'M') { 
                                        // manda el bloque a memoria
                                        int posIni = bloque * 4;
                                        for (int i = posIni; i < posIni+4; i++)
                                            memoriaPrincipal[i] = Convert.ToInt32(cache[i-posIni, bloque%4]); 
                                    }
                                }
                            }finally{
                                Monitor.Exit(bloqueadorC);
                            }
                        }else{ // si no logra bloquear el segundo recurso, libera el primero.
                            contador++;
                            bloqueoAdquirido = false;
                            Monitor.Exit(bloqueadorDir);
                        }

                    }else
                        contador++; // aqui aumenta el reloj
                }finally{
                    if (bloqueoAdquirido){
                        Monitor.Exit(bloqueadorDir);
                    }
                }
            } // fin de while 
        }

         /*
         *Procesa las instrucciones (entero por entero) y las asigna a un vector que funciona como
         *memoria de instrucciones. El cual contiene las 75 instrucciones por hilo.
         *Recibe el nombre del archivo (hilo.txt) y un entero que funciona como puntero del gran vector 
         *memoriaInsrucciones.
         */
        public void llenarMemInstrucciones(string nombreArchivo, int frontera){
            string linea;
            System.IO.StreamReader archivo = new System.IO.StreamReader(nombreArchivo);
            while((linea = archivo.ReadLine()) != null ){
                string[] palabras = linea.Split(' ');
                for (int i = 0; i < 4; ++i){
                    memoriaInstrucciones[frontera++] = Convert.ToInt32(palabras[i]);
                }
            }
        }

        /*
         *Asigna las siguientes 75 instrucciones o 300 enteros del vector Global (memoriaInstrucciones)
         *al vector local del Procesador
         *Aumenta el contadorIGlobal para que no se asigne el mismo conjunto de instrucciones al siguiente Procesador,
         *funcionando como un Hilo++.
         */
        
        private int[] getInstrucciones(){
            int cantPalHilos = CANTIDADINSTRUCCIONES * 4; // cantidad de palabras de un hilo 
            int[] instruccionesProcesador = new int[cantPalHilos];
            int posFinalMem = contadorIGlobal * cantPalHilos + cantPalHilos; // ultima posicion del hilo actual
            int iLocal = 0;  // recorre el vector 
            for (int i = contadorIGlobal * cantPalHilos; i < posFinalMem; i++){
                instruccionesProcesador[iLocal++] = memoriaInstrucciones[i];
            }
            string nombre = listaRutas[contadorIGlobal].ToString();
            nombre = nombre.Substring(nombre.Length- 6);
            ventanaResultados.mostrarHiloActual(Thread.CurrentThread.Name, nombre);
            contadorIGlobal++;
            return instruccionesProcesador;
        }

        /*
         *Retorna el número de bloque de una posición de memoria
         */
        private int getBloque(int posicion) {
            return posicion/4;
        }

        //Retorna el adminisitrador de un bloque específico.
        private int getAdministrador(int bloque){
            return bloque/8;
        }

        // despliega la memoria
        private void mostrarMemoria(){
            String mem1 = "";
            String mem2 = "";
            String mem3 = "";
            // ----------- memoria P1 --------------------
            mem1 += memoriaPrincipal[0];
            for (int i = 1; i < 31; i++){
                if (i % 4 == 0){
                    if (i == 16)
                        mem1 += "\r\n" + memoriaPrincipal[i];
                    else
                        mem1 += " -- " + memoriaPrincipal[i];
                }else{
                    mem1 += " " + memoriaPrincipal[i] ;
                }
            }
            mem1 += " " + memoriaPrincipal[31];
            // ----------- memoria P2 --------------------
            mem2 += memoriaPrincipal[32];
            for (int i = 33; i < 63; i++){
                if (i % 4 == 0){
                    if (i == 48)
                        mem2 += "\r\n" + memoriaPrincipal[i];
                    else
                        mem2 += " -- " + memoriaPrincipal[i];
                }else{
                    mem2 += " " + memoriaPrincipal[i] ;
                }
            }
            mem2 += " " + memoriaPrincipal[63];
            // ----------- memoria P3 --------------------
            mem3 += memoriaPrincipal[64];
            for (int i = 65; i < 95; i++){
                if (i % 4 == 0){
                    if (i == 80)
                        mem3 += "\r\n" + memoriaPrincipal[i];
                    else
                        mem3 += " -- " + memoriaPrincipal[i];
                }else{
                    mem3 += " " + memoriaPrincipal[i] ;
                }
            }
            mem3 += " " + memoriaPrincipal[95];
            ventanaResultados.mostrarMemorias(mem1, mem2, mem3);
        }

        // despliega las caches
        private void mostrarCaches() {
            string cache1 = "";
            string cache2 = "";
            string cache3 = "";
            for (int i = 0; i < cacheP1.GetLength(0); i++){
                for (int j = 0; j < cacheP1.GetLength(1); j++){
                    cache1 += cacheP1[i, j] + " ";
                    cache2 += cacheP2[i, j] + " ";
                    cache3 += cacheP3[i, j] + " ";
                }
                cache1 += "\r\n";
                cache2 += "\r\n";
                cache3 += "\r\n";
            }
            resultados[contadorResultados].mostrarCaches(cache1, cache2, cache3);
        }

        // despliega los directorios
        private void mostrarDirectorios() {
            string dir1 = "";
            string dir2 = "";
            string dir3 = "";
            for (int i = 0; i < directorioP1.GetLength(0); i++){
                dir1 += i + " ";
                dir2 += i+8 + " ";
                dir3 += i+16 + " ";
                for (int j = 0; j < directorioP1.GetLength(1); j++){
                    dir1 += directorioP1[i, j] + " ";
                    dir2 += directorioP2[i, j] + " ";
                    dir3 += directorioP3[i, j] + " ";
                }
                dir1 += "\r\n";
                dir2 += "\r\n";
                dir3 += "\r\n";
            }
            resultados[contadorResultados].mostrarDirectorios(dir1, dir2, dir3);
        }

        // despliega los registros
        private void mostrarRegistros(int proc, int[] registros) {
            String reg = "";
            resultados[contadorResultados].escribirProcesador(""+proc);
            for (int i = 0; i < 32; i++){  
                reg += registros[i] + " ";
            }
            resultados[contadorResultados].escribirRegistros(reg);
        }

        // despliega la ventana con los datos anteriores
        private void mostrarResultados() {
            if(contadorResultados == cantidadHilos-1)
                resultados[contadorResultados].escribirFinal();
            resultados[contadorResultados].Visible = true;
            contadorResultados++;
        }             
    }
}
