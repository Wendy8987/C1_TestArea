import {
    AmbientLight,
    AxesHelper,
    DirectionalLight,
    GridHelper,
    PerspectiveCamera,
    Scene,
    WebGLRenderer,
    LineBasicMaterial,
    LineSegments,
    EdgesGeometry,
    Matrix4
} from "./node_modules/three/build/three.module.js";
import { OrbitControls } from "../Model_Library/node_modules/three/examples/jsm/controls/OrbitControls.js";
import { IFCLoader } from '../Model_Library/node_modules/web-ifc-three/IFCLoader';
// 创建Three.js场景
const scene = new Scene();

const element_windows = document.getElementById("element_windows");
//Object to store the size of the viewport
const size = {
    width: element_windows.offsetWidth,
    height: element_windows.offsetHeight,
};

// 创建摄像机（用户的视角）。
const aspect = size.width / size.height;
const camera = new PerspectiveCamera(75, aspect);
camera.position.z = 15;
camera.position.y = 13;
camera.position.x = 8;

// 创建场景的灯光
const lightColor = 0xffffff;

const ambientLight = new AmbientLight(lightColor, 0.5);
scene.add(ambientLight);

const directionalLight = new DirectionalLight(lightColor, 1);
directionalLight.position.set(0, 10, 0);
directionalLight.target.position.set(-5, 0, 0);
scene.add(directionalLight);
scene.add(directionalLight.target);

// 设置渲染器，获取HTML的画布。
const threeCanvas = document.getElementById("three-canvas");
const renderer = new WebGLRenderer({
    canvas: threeCanvas,
    alpha: true,
});

renderer.setSize(size.width, size.height);
renderer.setPixelRatio(Math.min(window.devicePixelRatio, 2));

//// 在场景中创建网格和坐标轴
//const grid = new GridHelper(50, 50);
//scene.add(grid);

const axes = new AxesHelper();
axes.material.depthTest = false;
axes.renderOrder = 1;
scene.add(axes);

// 创建轨道控制（用于导航场景）。
const controls = new OrbitControls(camera, threeCanvas);
controls.enableDamping = true;
controls.target.set(-2, 0, 0);

// 动画循环
const animate = () => {
    controls.update();
    renderer.render(scene, camera);
    requestAnimationFrame(animate);
};

animate();

// 根据浏览器的大小调整视口
element_windows.addEventListener("resize", () => {
    size.width = element_windows.offsetWidth;
    size.height = element_windows.offsetHeight;
    camera.aspect = size.width / size.height; 
    camera.updateProjectionMatrix();
    renderer.setSize(size.width, size.height);
});

//Sets up the IFC loading
const ifcModels = [];
const ifcLoader = new IFCLoader();
ifcLoader.ifcManager.setWasmPath("/wasm/");

const hexdata = document.getElementById("fileurl").value;

var byteArray = new Uint8Array(hexdata.length / 2);
for (var x = 0; x < byteArray.length; x++) {
    byteArray[x] = parseInt(hexdata.substr(x * 2, 2), 16);
}

var blob = new Blob([byteArray], { type: "application/octet-stream" });
var btn = document.getElementById("button_trigger")
var block = document.getElementById("modelbtn")
const modelURL = URL.createObjectURL(blob);
ifcLoader.load(modelURL, (ifcModel) => {
    loadIFC(modelURL)
    block.style.display = "none";
    btn.style.display = "block";
});
async function loadIFC(url) {
    const firstModel = Boolean(ifcModels.length === 0);
    ifcLoader.ifcManager.applyWebIfcConfig({
        COORDINATE_TO_ORIGIN: firstModel,
        USE_FAST_BOOLS: false
    });
    const ifcModel = await ifcLoader.loadAsync(url);
    if (firstModel) {
        const matrixArr = await ifcLoader.ifcManager.ifcAPI.GetCoordinationMatrix(ifcModel.modelID);
        const matrix = new Matrix4().fromArray(matrixArr);
        ifcLoader.ifcManager.setupCoordinationMatrix(matrix);
    }
    var geo = new EdgesGeometry(ifcModel.geometry); // or WireframeGeometry( geometry )
    var mat = new LineBasicMaterial({ color: 0x113285, linewidth: 2 });
    var wireframe = new LineSegments(geo, mat);
    scene.add(wireframe);

    ifcModels.push(ifcModel);
    scene.add(ifcModel);
    //Magic function to load Data
    window.Manager = ifcLoader.ifcManager
}