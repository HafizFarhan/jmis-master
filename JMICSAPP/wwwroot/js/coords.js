function pr(e) { console.log(e); }
/*******************************************************************************
   Biblioteca que converte campos html em campos específicos para armazenar,
   formatar e exibir latitudes e longitudes.

   -----------------------------------------------------------------------------
   VERSION 0.2 28/01/2017
   -----------------------------------------------------------------------------
   COPYRIGHT
   -----------------------------------------------------------------------------
   EXAMPLES

   coords.init( 'input.coords' );

   -----------------------------------------------------------------------------
   TODO
    - Verificar se um campo maior tem valor com decimal, mesmo que os campos
      menores estejam preenchidos.
    - Adicionar validações. Ex.: Se os minutos ou segundos são maiores que 60.
    - [OK] Criar possibilidade de alterar o formato enviado para o servidor.
        - Pode criar uma propriedade com o nome onSaveFormat que
        armazenaria o formato de salvamento do dados. Esse formato poderia ser
        cadastrado com um método setSaveFormat() recebendo as configurações
        baseada em initialOptions e no método options(). Esse valor será
        modificado ao alterar os dados e a funçãoo onChange fará essa verificação
        ao alterar.
    - Converter lat/long do UTM
    - Adicionar botão ao lado direito para abrir um popup com um mapa para
      selecionar a latitude/longitude
    - Calcular distância entre 2 coordenadas
    - Transformar campo de texto em um autocomplete para endereços, sendo possível
      converter o endereço digitado para preencher os campos de latitude e longitude
    - [OK] Permitir copiar as coordenadas para a área de transferência só apertando
      ctrl+c em qualquer um dos campos

 *******************************************************************************/
var coords = {

    /**
     * @var
     * Opções gerais, usado para exibição dos inputs e para conversão entre formatos
     */
    initialOptions: {
        'spaces': true,  //Spaces between parts
        'degrees': true,  //Show degrees. Always true
        'minutes': true,  //Show minutes.
        'seconds': true,  //Show seconds. False if minutes is false too
        'degreeIndicator': '°',   //Degree component indicator
        'minuteIndicator': "'",   //Minute component indicator
        'secondIndicator': '"',   //Second component indicator
        'showSign': false, //Configure to show sign at start
        'showCompassDirection': true,  //Configure to show compass direction at end
        'decimalSeparator': '.',   //Last component's decimal separator
        'decimalPlaces': 5, //Arredondar o último componente para um número de casas definido
        //@todo: permitir renomear as direções exibidas
        //'compassDirections'     : { 'north': 'N', 'east': 'E', 'west': 'W', 'south':'S' }, //change compass directions at exibition
        //@todo: Exibir um ícone à direita do campo que permita abrir uma popup e selecionar a localização no mapa
        //'showIcon'              : false, //Show 'openMap' icon at right of input

        //Recalculate widths
        'recalculateWidth': true,
        'pixelsBychars': 8,

        'saveFormatOptions': null,

        //Messages
        'pasteErrorMessage': 'The pasted texto isn\'t a valid coordinate',

    },
    /************************************************************
     *
     *  BASICS
     *
     ************************************************************/

    /**
     * init()
     * Inicia a biblioteca a partir de um seletor jquery
     *
     * @version 0.1 21/01/2017 Initial
     *          0.2 22/01/2017 Removido jQuery
     *          0.3 28/01/2017
     *
     * @example
     *      //Usando uma class
     *      coords.init( '.coordinates' );
     *      //Usando o nome dos campos
     *      coords.init( '[name="latitude"], [name="longitude"]' )
     *
     * @param string strSelector Seletor dos inputs que receberão a transformação.
     *                           O seletor precisa ser compatível com document.querySelectorAll()
     * @param object options Configurações de exibição dos campos
     * @return
     */
    init: function (strSelector, options) {
        var $obj = this;
        this.initialOptions = this.options(options);

        //Se não existir opções para o formato de salvamento, use o padrão
        if (this.saveFormatOptions == null) {
            this.setSaveFormat(this.initialOptions);
        }

        var inputs = document.querySelectorAll(strSelector);

        for (i = 0; i < inputs.length; i++) {
            $obj.makeAllEverythingAndOthers(inputs[i]);
        }

        //Return object
        return this;
    },

    /************************************************************
     *
     *  DOM HANDLE
     *
     ************************************************************/

    /**
     * makeAllEverythingAndOthers()
     * Recebe um objeto individual e faz a transformação nele
     * - os campos + e - e as direções NEWS, serão colocadas com campos select
     * - precisa gerar uma nova class informando o formato que está sendo usado
     * -
     * @version 0.1 21/01/2017 Initial
     *          0.2 22/01/2017 Removido jquery
     *          0.3 23/01/2017 Removido o parâmetro options. O método recebe
     *                         direto da propriedade da classe.
     *          0.4 25/01/2017 Adicionado suporte ao auto preenchimento quando o
     *                         valor já existir no campo
     *
     * @example
     *
     * @param object objInput
     * @return
     */
    makeAllEverythingAndOthers: function (objInput) {
        var options = this.initialOptions;
        //Esconde o input principal
        objInput.style.display = 'none';

        var inputName = objInput.getAttribute('name');
        var showIconClass = this.options.showIcon ? 'coords-show-icon-map' : '';

        //Criando o container
        var objContainer = document.createElement('DIV');
        objContainer.className = 'coords-container ' + showIconClass + ' ';
        objInput.parentNode.insertBefore(objContainer, objInput.nextSibling);

        //Verifica se deve exibir o sinal à esquerda
        if (options.showSign == true) {
            this.createSelect(objContainer, inputName, 'signal', ['', '+', '-']);
        }

        // Verifica as opções e cria os demais inputs
        //Sempre exibido
        this.createElement(objContainer, inputName, 'degrees', options.degreeIndicator)

        //Exibindo quando minutes=true e quando seconds também for
        if (options.minutes == true || options.seconds == true) {
            this.createElement(objContainer, inputName, 'minutes', options.minuteIndicator)
        }

        if (options.seconds == true) {
            this.createElement(objContainer, inputName, 'seconds', options.secondIndicator)
        }

        //Verifica se deve exibir o sinal à esquerda
        if (options.showCompassDirection == true) {
            switch (inputName) {
                case "Latitude":
                    this.createSelect(objContainer, inputName, 'compass', ['N', 'W']);
                    break;
                case "Longitude":
                    this.createSelect(objContainer, inputName, 'compass', ['E', 'S']);
                    break;
                default:
                    this.createSelect(objContainer, inputName, 'compass', ['E', 'N', 'S', 'W']);
                    break;
            }
        }

        //Ativando o preenchimento dos campos
        this.setInitialValues(objInput);
    },

    /**
     * createElement()
     * Cria os elementos com javascript puro
     *
     * @version 0.1 22/01/2017 Initial
     *
     * @param object objContainer Objeto do container dos campos
     * @param string inputName Nome do input original
     * @param string coordsType Tipo da parte da coordenada
     * @param string indicator Caractere usado como legenda do campo
     * @return void
     */
    createElement: function (objContainer, inputName, coordsType, indicator) {
        var input = document.createElement('INPUT');
        input.setAttribute('type', 'text');
        input.setAttribute('data-coords-type', coordsType);
        input.setAttribute('name', inputName + '_coords_' + coordsType);
        input.setAttribute('class', 'coords-input coords-' + coordsType);

        objContainer.appendChild(input);
        this.eventsHandler(input);

        var span = document.createElement('SPAN');
        span.setAttribute('class', 'coords-indicator coords-' + coordsType);
        span.innerHTML = indicator;
        objContainer.appendChild(span);
    },

    /**
     * createSelect()
     * Cria os selects
     *
     * @version 0.1 23/01/2017 Initial
     *
     * @param object objContainer Objeto do container dos campos
     * @param string inputName Nome do input original
     * @param string selectType Tipo do select: 'sign' ou 'directions'
     * @param array data Array com os dados do select: [ 'N', 'E', 'W', 'S' ]
     * @return void
     */
    createSelect: function (objContainer, inputName, selectType, data) {
        var select = document.createElement('SELECT');
        select.setAttribute('name', inputName + '_' + selectType);
        select.setAttribute('class', 'coords-select coords-' + selectType);
        select.setAttribute('data-coords-type', selectType);

        objContainer.appendChild(select);

        for (var i = 0; i < data.length; i++) {
            var opt = document.createElement('option');
            opt.innerHTML = data[i];
            opt.value = data[i];
            select.appendChild(opt);
        }

        this.eventsHandler(select);
    },

    /**
     * batchValues()
     * Adiciona valores a todos os campos de uma vez. Usado em onPaste e ao carregar
     * a biblioteca com um valor predefinido
     *
     * @version 0.1 23/01/2017 Initial
     *          0.2 25/01/2017 O primeiro parâmetro agora recebe o objeto do input
     *
     * @param object $objInput Objeto do input original
     * @param string strCoord Coordenada que será parseada e colocada nos sub-campos
     * @return void
     */
    batchValues: function ($objInput, strCoord) {
        var parse = this.parse(strCoord);
        if (parse == false) return false;
        //pr(strCoord)
        //pr(this.initialOptions.decimalPlaces)
        var children = $objInput.nextSibling.children

        for (i = 0; i < children.length; i++) {

            switch (children[i].getAttribute('data-coords-type')) {
                case 'signal':
                    children[i].value = parse.signal;
                    break;
                case 'compass':
                    children[i].value = parse.compass;
                    break;
                case 'degrees':
                    children[i].value = Number.isInteger(parse.degrees) === true ?
                        parse.degrees : parse.degrees.toFixed(this.initialOptions.decimalPlaces);
                    break;
                case 'minutes':
                    children[i].value = Number.isInteger(parse.minutes) === true ?
                        parse.minutes : parse.minutes.toFixed(this.initialOptions.decimalPlaces);
                    break;
                case 'seconds':
                    children[i].value = Number.isInteger(parse.seconds) === true ?
                        parse.seconds : parse.seconds.toFixed(this.initialOptions.decimalPlaces);
                    break;
            }
        }

        return true;
    },

    /**
     * setInitialValues()
     * Verifica se o campo original possui um valor prévio e atualiza os sub-campos
     * para receber a informação.
     *
     * @version 0.1 25/01/2017 Initial
     *
     * @param object objInput objeto da input original
     * @return void
     */
    setInitialValues: function (objInput) {
        if (typeof objInput == 'object' && objInput.tagName == 'INPUT' && objInput.value !== '') {
            this.batchValues(objInput, objInput.value);
            this.calculateWidths(objInput);
            //Altera o próprio valor para o formato definido para salvamento
            objInput.setAttribute('value', this.convert(objInput.value, this.saveFormatOptions));
        }
    },

    /**
     * setSaveFormat()
     * Define o formato que os dados serão salvos
     *
     * @version 0.1 28/01/2017 Initial
     *
     * @param object options Opções de configuração
     * @return void
     */
    setSaveFormat: function (options) {
        this.saveFormatOptions = this.options(options);
    },

    /**
     * Recalcula o tamanho dos campos, sendo 8px para cada caractere
     *
     * @version 0.1 23/01/2017 Initial
     *          0.2 25/01/2017 O parâmetro foi alterado do evento para o input original
     *
     * @param object objInput Input original
     * @return void
     */
    calculateWidths: function (objInput) {
        if (this.initialOptions.recalculateWidth !== true) return;

        var pixelsBychars = this.initialOptions.pixelsBychars || 8;
        var children = objInput.nextSibling.children;
        //pr(objInput)
        for (i = 0; i < children.length; i++) {
            if (children[i].getAttribute('class').search('coords-input') !== -1) {
                children[i].style.width = ((children[i].value.length + 1) * pixelsBychars) + 'px';
            }
            else if (children[i].getAttribute('class').search('coords-select') !== -1) {
                children[i].style.width = '35px';
            }
        }
    },


    /************************************************************
     *
     *  EVENTS HANDLER
     *  Gerencia os eventos dos campos criados
     *
     ************************************************************/



    /**
     * Gera os eventos
     *
     * @version 0.1 22/01/2017 Initial
     *
     *** Eventos
     * @todo
     * - [OK] aumenta o tamanho dos campos de acordo com os caracteres
     * - verifica se os minutos ou segundos são maiores que 60
     * - verifica se os graus são maiores que 180 (long), Lat vão até 90 (http://www.geomidpoint.com/latlon.html)
     *   converte para o formato definido
     * - verifica ao copiar um dos valores qual o formato definido e copia
     *   para a área de transferência o valor formatado
     * - [OK] verifica ao colar um valor em qualquer campo e auto preenche os demais caso seja uma coordenda válida.
     * - [OK] seleciona todo o conteúdo ao entrar no campo
     * - [OK] Ao atualizar os campos, atualizar o valor do input oculto que será enviado
     * - verificar se fica melhor usar campos P editáveis ao invés de novos campos de formulário. Os campos P ficariam melhor do que usar JavaScript pra ficar aumentando e diminuindo o tamanho dos input.
     *
     * @param object objInput Campo que receberá o evento
     * @return void
     */
    eventsHandler: function (objInput) {
        var $this = this;

        //Manipula o código ao colar o resultado
        objInput.addEventListener('paste', function (evnt) { $this.onPaste(evnt, $this) });
        //Manipula o código que seleciona todo o texto ao entrar
        objInput.addEventListener('focus', function (evnt) { $this.onFocus(evnt, $this) });
        //Manipula o código ao digitar uma tecla
        objInput.addEventListener('keydown', function (evnt) { $this.onKeydown(evnt, $this) });
        //Evento que atualiza o valor do campo oculto ao realizar alterações
        objInput.addEventListener('change', function (evnt) { $this.onChange(evnt, $this) });

        objInput.addEventListener('copy', function (evnt) { $this.onCopy(evnt, $this) });
    },

    /**
     * onCopy()
     * Copia o valor da coordenada para a área de transferência de acordo com o
     * formato definido para salvamento
     *
     * @version 0.1 28/01/2017 Initial
     *
     * @param object evnt Objeto do evento
     * @param object $this Objeto da classe
     * @return void
     */
    onCopy: function (evnt, $this) {
        //console.log(e);
        //console.log(objInput)
        evnt.clipboardData.setData('text/plain', $this.inputToString(evnt.target.parentNode.previousSibling));
        evnt.preventDefault();
    },

    /**
     * onChange()
     * Atualiza o valor do campo oculto ao alterar os campos criados
     *
     * @version 0.1 24/01/2017 Initial
     *          0.2 25/01/2017 Correção na atualização do valor do campo original
     *
     * @param object evnt Objeto do evento
     * @param object $this Objeto da classe
     * @return void
     */
    onChange: function (evnt, $this) {
        var $container = evnt.target.parentNode;
        var $children = $container.children;
        var $input = $container.previousSibling;
        var strCoord = '';

        for (i = 0; i < $children.length; i++) {
            if ($children[i].tagName == 'INPUT' || $children[i].tagName == 'SELECT') {
                strCoord += (typeof $children[i].value == 'undefined' ? '0' : $children[i].value) + ' ';
            }
        }

        $input.setAttribute('value', $this.convert(strCoord, $this.saveFormatOptions));
    },


    /**
     * onKeydown()
     * Ao pressionar uma tecla
     *
     * @version 0.1 22/01/2017 Initial
     *          0.2 23/01/2017 Adicionado o objeto da classe como parâmetro
     *          0.3 25/01/2017 Repassa o objeto do input original como parâmetro de calculateWidths()
     *          0.4 28/01/2017 Só recalcula os tamanhos se digitar um número ou um ponto ou uma vírgula
     *
     * @param object evnt Objeto do evento
     * @param object $this Objeto da classe
     * @return void
     */
    onKeydown: function (evnt, $this) {
        //pr(evnt.keyCode);
        if (evnt.target.tagName == 'INPUT' && (evnt.keyCode >= 48 && evnt.keyCode <= 57 || evnt.keyCode == 188 || evnt.keyCode == 190)) {
            $this.calculateWidths(evnt.target.parentNode.previousSibling);
        }
    },

    /**
     * onFocus()
     * Seleciona todo o texto ao entrar no campo
     *
     * @version 0.1 22/01/2017 Initial
     *          0.2 23/01/2017 Adicionado o objeto da classe como parâmetro
     *
     * @param object evnt Objeto do evento
     * @param object $this Objeto da classe
     * @return void
     *
     */
    onFocus: function (evnt, $this) {
        evnt.stopPropagation();
        evnt.preventDefault();

        if (evnt.target.tagName == 'INPUT') {
            evnt.target.select();
        }
    },

    /**
     * onPaste()
     * Gerencia o que acontece quando colamos um valor dentro dos campos
     * - Lê o valor, normaliza, dá um parse e separa os componentes nos
     *   campos de acordo com o formato desejado.
     *
     * @version 0.1 22/01/2017 Initial
     *
     * @param object evnt  Objeto do evento
     * @param object $this Objeto da classe
     * @return void
     */
    onPaste: function (evnt, $this) {
        var clipboardData, pastedData;

        // Stop data actually being pasted into div
        evnt.stopPropagation();
        evnt.preventDefault();

        // Get pasted data via clipboard API
        clipboardData = evnt.clipboardData || window.clipboardData;
        pastedData = clipboardData.getData('Text');

        if ($this.batchValues(evnt.target.parentNode.previousSibling, pastedData) === false) {
            alert($this.initialOptions.pasteErrorMessage)
        }

        //Recalcula os tamanhos
        $this.calculateWidths(evnt.target.parentNode.previousSibling);
    },

    /************************************************************
    *
    * MATH AND PATTERNS
    * Handles calculus and formats to add support into others functions
    *
    ************************************************************/


    /**
     * Mescla as opções de acordo com as regras definidas para evitar distorções.
     * * Regras
     * - degree é sempre true;
     * - minute é true caso seconds também o seja
     *
     * @version 0.1 22/01/2017 Initial
     *          0.2 23/01/2017 Mescla as opções definidas dentro das originais
     *          0.3 25/01/2017 Não mescla mais as opções originais
     *
     * @param  object options Opções que serão mescladas com as opções iniciais
     * @return object
     */
    options: function (options) {
        if (typeof options == 'undefined') return this.initialOptions;
        //basic rules
        //degrees is always true
        options.degrees = true;
        //seconds is false if minutes is false too
        if (options.minutes == false) options.seconds = false;
        //return
        options = Object.assign({}, this.initialOptions, options);

        return options;
    },

    /**
     * normalize
     * Tenta deixar a expressão em um formato padronizado
     * - Remove espaços duplicados
     * - Substitui ' ʹ ʼ ˈ ́  ׳ ′ꞌ  por '
     * - Substitui '' por "
     * - Substitui " „ “ ” por "
     * - Substitui ° ˚ ̊  ⁰ ∘ ◦ ॰ o por °
     * - Substitui , por .
     *
     * @version 0.1 21/01/2017 Initial
     *
     * Fonte dos caracteres: http://www.fileformat.info/info/unicode/char/00b0/index.htm
     *
     * @todo: substituir os caracteres pelos seus correspondentes no unicode
     *        http://www.w3schools.com/jsref/jsref_regexp_unicode_hex.asp
     * @param string strCoord Texto com a coordenada em qualquer valor
     * @return String Coordenadas formatadas
     */
    normalize: function (strCoord) {
        //strCoord = strCoord
        //.replace(/(\xb0|\x2da|\x30a|\x2070|\x2218|\x25e6|\x970|\x6f)/g, '°')     // ° ˚ ̊  ⁰ ∘ ◦ ॰ º o
        //.replace(/[\xb0\x2da\x30a\x2070\x2218\x25e6\x970\xba\x6f]/g, '°')     // ° ˚ ̊  ⁰ ∘ ◦ ॰  º o
        // .replace(/[\x27\x2b9\x2bc\x2c8\x301\x5f3\x2032\xa78c]/g, "'") // ' ʹ ʼ ˈ ́  ׳ ′ꞌ
        // .replace(/[\x22\x201e\x201c\x201d]/g, '"') //  " „ “ ”

        // pr('1) '+strCoord)
        strCoord = strCoord.replace(/\s{2,}/g, " ") //Remove espaços duplos
        // pr('2) '+strCoord)
        strCoord = strCoord.replace(/[°˚⁰∘◦॰ºo]+/g, '°')
        // pr('3) '+strCoord)
        strCoord = strCoord.replace(/['ʹʼˈ׳′ꞌ]{1}/g, "'")
        // pr('4) '+strCoord)
        strCoord = strCoord.replace("''", '"') //two double quotes
        // pr('5) '+strCoord)
        strCoord = strCoord.replace(/["„“”]+/g, '"')
        // pr('6) '+strCoord)
        strCoord = strCoord.replace(/\,+/g, '.') //transformando vírgulas em pontos
        // pr('7) '+strCoord)
        return strCoord;
    },

    /**
     * parse()
     * Pega os componentes da coordenada
     *
     * @version 0.1 21/01/2017 Initial
     *          0.2 23/01/2017 Parse agora converte todos os valores para DMS,
     *                         independentemente da entrada.
     *
     * @param strCoord
     * @return
     */
    parse: function (strCoord) {
        //Iniciando o parsing normalizado
        strCoord = this.normalize(strCoord);

        //V0.1
        //var pattern = /([NEWS]{1}|[-+]{1})?([0-9,\.]+°)([0-9,\.]+')?([0-9,\.]+")?([NEWS]{1})?/i;
        //V0.2 - 22/01/2017 - Adicionado suporte ao formato "12 24 56"
        var pattern = /([NEWS]{1}|[-+]{1})?\s*([0-9,\.]+\s*°?)\s*([0-9,\.]+\s*'?)?\s*([0-9,\.]+\s*"?)?\s*([NEWS]{1})?/i

        var parts = pattern.exec(strCoord);
        // pr(parts)
        if (parts == null) return false;

        var compassDirections = ['N', 'E', 'W', 'S'];

        //Só tem como determinar qual a direção se for informada, pois em casos negativos podem
        //tanto ser W como S, assim como o positivo podem ser N e E, dependendo se é latitude ou longitude
        var hasCompass = compassDirections.indexOf(parts[1]) !== -1 ? parts[1].toUpperCase() : (compassDirections.indexOf(parts[5]) !== -1 ? parts[5] : false)
        var hasSignal = parts[1] == '-' || ['S', 'W'].indexOf(hasCompass) !== -1 ? '-' : '+';

        var degrees = typeof parts[2] !== 'undefined' ? parseFloat(parts[2]) : 0;
        var minutes = typeof parts[3] !== 'undefined' ? parseFloat(parts[3]) : 0;
        var seconds = typeof parts[4] !== 'undefined' ? parseFloat(parts[4]) : 0;

        //se minutos for zero e segundos for zero mas tiver casas decimais nos graus, divida os valores com os menores
        if (minutes === 0 && seconds === 0 && (degrees !== parseInt(degrees))) {
            minutes = (degrees - parseInt(degrees)) * 60;
            degrees = parseInt(degrees);
        }

        //se segundos for zero mas tiver casas decimais nos minutos, divida os valores com ele
        if (seconds === 0 && (minutes !== parseInt(minutes))) {
            seconds = (minutes - parseInt(minutes)) * 60;
            minutes = parseInt(minutes)
        }

        return {
            signal: hasSignal,
            compass: hasCompass,
            degrees: degrees,
            minutes: minutes,
            seconds: seconds,
        };
    },

    /**
     * Converte uma coordenada para float a partir de uma string em qualquer formato
     *
     * @version 0.1 22/01/2017 Initial
     *
     * @param string strCoord
     * @return
     */
    stringToDecimal: function (strCoord) {
        return this.convert(strCoord, {
            'degrees': true, 'minutes': false, 'seconds': false,
            'showSign': true, 'spaces': false, 'degreeIndicator': '',
            'showCompassDirection': false
        });
    },

    /**
     * convert()
     * Converte uma coordenada em float para o formado DD,DDD°
     *
     * @version 0.1 22/01/2017 Initial
     *          0.2 25/01/2017 Conversão realizada por parseObjectToString()
     *
     * @example
     *
     * @param string strCoord Coordenada em qualquer formato de texto
     * @param object options  Objeto de configuração do retorno da conversão
     * @return string
     */
    convert: function (strCoord, options) {
        return this.parseObjectToString(this.parse(strCoord), options);
    },

    /**
     * inputToString()
     * A partir do objeto input original, devolve o valor dos sub inputs no formato definido em options
     *
     * @version 0.1 25/01/2017 Initial
     *
     * @param (object|input) input Input object ou seletor que localize este input. O seletor precisa levar a somente um objeto
     * @param object options Opções para o retorno da string
     * @return string Contendo os dados do objeto no formato definido em options
     */
    inputToString: function (input, options) {
        //Convertendo para objeto caso "input" seja um seletor
        input = (typeof input == 'string') ? document.querySelector(input) : input;

        //Ajustando as opções
        options = this.options(options);

        //Se é um objeto de um input
        if (input.tagName !== 'INPUT') { console.log('"input" parameter isn\'t a valid input object'); return false; }

        var $container = input.nextSibling;
        var $children = $container.children;

        var strCoord = '';

        for (i = 0; i < $children.length; i++) {
            if ($children[i].tagName == 'INPUT') {
                strCoord += ($children[i].value == '' ? 0 : $children[i].value);
            }
            else if ($children[i].tagName == 'SELECT' && $children[i].value !== "") {
                strCoord += $children[i].value;
            }
            else {
                continue;
            }
            strCoord += ' ';
        }

        return this.convert(strCoord, options);
    },

    /**
     * parseObjectToString()
     * A partir do objeto retornado por parse(), retorna uma string de acordo com
     * o formato definido em options
     *
     * @version 0.1 25/01/2017 Initial
     *
     * @param  object objParse Objeto retornado por parse()
     * @param  object options  Objeto de configurações
     * @return
     */
    parseObjectToString: function (objParse, options) {
        var newOptions = this.options(options);

        if (newOptions.seconds == false) {
            objParse.minutes = objParse.minutes + (objParse.seconds / 60);
            objParse.seconds = false;
        }

        if (newOptions.minutes == false) {
            objParse.degrees = objParse.degrees + (objParse.minutes / 60);
            objParse.minutes = false;
        }
        var spaces = newOptions.spaces ? ' ' : '';

        return (
            //Exibindo o sinal caso a opção esteja definida
            (newOptions.showSign ? objParse.signal + spaces : '')
            //exibindo a parte dos graus junto com a opção do indicador
            + objParse.degrees + newOptions.degreeIndicator
            //   //Exibindo a parte dos minutos
            + (newOptions.minutes == true ? spaces + objParse.minutes + newOptions.minuteIndicator : '')
            //   //Exibindo a parte dos segundos
            + (newOptions.seconds == true ? spaces + objParse.seconds + newOptions.secondIndicator : '')
            //   //imprimindo a direção da bússola
            + (newOptions.showCompassDirection && objParse.compass ? spaces + objParse.compass : '')
            // + ( newOptions.showCompassDirection && objParse.compass ? spaces + newOptions.compassDirections.indexOf( objParse.compass ) : '' )
            //   //substituindo os pontos e vírgulas pela opção definida
        ).replace(/[,\.]+/g, newOptions.decimalSeparator).trim();

    },

};

/**
 * Cria a função do jQuery
 */
if (window.jQuery) {
    jQuery.fn.extend({
        'coords': function (options) {
            coords.init(this.selector, options);
            return this;
        },
    });

    $(function () {
        //Exemplo
        //Inicia a biblioteca via jQuery
        // $('input[type="coords"]').coords( { minutes: true, seconds: true } );
        //$('input.coords').coords( { minutes: true, seconds: true } );
    });

}
