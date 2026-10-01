import {
  PrepareRecoveryRequest,
  ResolveRecoveryRequest,
  DistractionReason,
  FocusSessionReflection,
  FocusSessionsClient,
  InterventionStrategy,
  RecordFocusSessionReflectionRequest,
  ReportDistractionRequest,
  StartFocusSessionRequest,
  type DistractionReportDto,
  type FocusSessionDto,
  type FocusSessionHistoryItemDto,
} from '../../../web-api-client';

const focusSessionsClient = new FocusSessionsClient();

export interface StartFocusSessionInput {
  taskId: number;
  taskStartPlanId: number;
}

export async function startFocusSession(
  input: StartFocusSessionInput,
): Promise<FocusSessionDto> {
  const session = await focusSessionsClient.startFocusSession(
    new StartFocusSessionRequest(input),
  );

  if (session.id === undefined) {
    throw new Error('The focus session response did not include an id.');
  }

  return session;
}

export function getFocusSession(sessionId: number): Promise<FocusSessionDto> {
  return focusSessionsClient.getFocusSession(sessionId);
}

export function getFocusSessionHistory(): Promise<FocusSessionHistoryItemDto[]> {
  return focusSessionsClient.getFocusSessionHistory();
}

export function completeFocusSession(sessionId: number): Promise<FocusSessionDto> {
  return focusSessionsClient.completeFocusSession(sessionId);
}

export function recordFocusSessionReflection(
  sessionId: number,
  reflection: FocusSessionReflection,
): Promise<FocusSessionDto> {
  return focusSessionsClient.recordFocusSessionReflection(
    sessionId,
    new RecordFocusSessionReflectionRequest({ reflection }),
  );
}

export function reportDistraction(
  sessionId: number,
  reason: DistractionReason,
  language: string,
): Promise<DistractionReportDto> {
  return focusSessionsClient.reportDistraction(
    sessionId,
    new ReportDistractionRequest({ reason, language }),
  );
}

export { DistractionReason, FocusSessionReflection, InterventionStrategy };
export type {
  DistractionReportDto,
  FocusSessionDto,
  FocusSessionHistoryItemDto,
};

export function prepareRecovery(sessionId: number, distractionId: number, input: PrepareRecoveryRequest) {
  return focusSessionsClient.prepareRecovery(sessionId, distractionId, input);
}

export function resolveRecovery(sessionId: number, distractionId: number, input: ResolveRecoveryRequest) {
  return focusSessionsClient.resolveRecovery(sessionId, distractionId, input);
}

export {
  RecoveryChoice, RecoveryInterventionType, RecoveryRequirement, RecoveryResolution,
  PrepareRecoveryRequest, ResolveRecoveryRequest,
} from '../../../web-api-client';
