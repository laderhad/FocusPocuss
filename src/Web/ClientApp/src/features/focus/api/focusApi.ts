import {
  DistractionReason,
  FocusSessionReflection,
  FocusSessionsClient,
  InterventionStrategy,
  RecordFocusSessionReflectionRequest,
  ReportDistractionRequest,
  StartFocusSessionRequest,
  type DistractionReportDto,
  type FocusSessionDto,
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
): Promise<DistractionReportDto> {
  return focusSessionsClient.reportDistraction(
    sessionId,
    new ReportDistractionRequest({ reason }),
  );
}

export { DistractionReason, FocusSessionReflection, InterventionStrategy };
export type { DistractionReportDto, FocusSessionDto };
