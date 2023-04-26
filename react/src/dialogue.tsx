import { useGlobals, useReactiveValue } from '@reactunity/renderer';

import './index.scss';

export default function Dialogue(): React.ReactNode {
    const globals = useGlobals();
    const dialogueSpeaker = useReactiveValue(globals.dialogueSpeaker);
    const dialogueText = useReactiveValue(globals.dialogueText);

    return <view className="black-bar">
        <view className="content">
            <h1 className="title">{dialogueSpeaker}</h1>
            <view className="gradient-rule"></view>
            <p className="message">{dialogueText}</p>
        </view>
    </view>;
}